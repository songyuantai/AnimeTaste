using AnimeTaste.Core.Const;
using AnimeTaste.Core.Utils;
using AnimeTaste.Model;
using AnimeTaste.Service.Cache;
using AnimeTaste.ViewModel;
using JikanDotNet;
using SqlSugar;
using Anime = AnimeTaste.Model.Anime;
using Season = AnimeTaste.Model.Season;

namespace AnimeTaste.Service
{
    public class SeasonService(ISqlSugarClient db, RedisService redis, IJikan jikan, AnimeService animeService)
    {
        public async Task<List<Season>> GetOrAddSeasonList()
        {
            const string SeasonKey = "animeSeason";
            var seasons = GetAllSeasons();
            var list = await redis.ListGetAsync<Season>(SeasonKey);
            //缓存命中
            if (seasons.Count == list.Count && seasons.Last()?.StartDate == list.Last()?.StartDate)
            {
                return list;
            }

            list = await db.Queryable<Season>().ToListAsync();
            for (var i = 0; i < seasons.Count; i++)
            {
                var season = seasons[i];
                if (i < list.Count && list[i].SeasonNumber == season.SeasonNumber)
                {
                    continue;
                }
                await db.Insertable(season).ExecuteCommandIdentityIntoEntityAsync();
                if (i < list.Count)
                    list[i] = season;
                else
                    list.Add(season);
            }

            await redis.ListReplaceAsync(SeasonKey, list);

            return list;
        }

        public const string AnimeScheduleList = "AnimeScheduleList";

        public async Task<List<AnimeScheduleInfo>> GetOrAddSeasonAnimeList(int seasonId, int dayOfWeek)
        {
            var key = $"{AnimeScheduleList}:{seasonId}:{dayOfWeek}";
            if (seasonId == 0 || dayOfWeek < 0 || dayOfWeek > 6) return [];

            //var list = await redis.ScanEntityList<Anime>(key);
            var list = await redis.ListGetAsync<AnimeScheduleInfo>(key);
            if (list.Count == 0)
            {
                var animes = await db.Queryable<Anime>()
                    .Where(m => m.SeasonId == seasonId && m.BroadcastDay == dayOfWeek)
                    .ToListAsync();

                if (animes.Count == 0)
                {
                    list = await AddOrUpdateSeasonAnimes(seasonId, dayOfWeek);
                }
                else
                {
                    var animeIds = animes.Select(m => m.Id).ToList();
                    var animeIdCollceted = await db.Queryable<AnimeCollection>().Where(m => animeIds.Contains(m.AnimeId)).Select(m => m.AnimeId).ToArrayAsync();
                    var animeIdCollcetedSet = animeIdCollceted.ToHashSet();
                    foreach (var anime in animes)
                    {
                        var images = await db.Queryable<AnimeImage>().Where(m => m.AnimeId == anime.Id).ToListAsync();
                        var item = ToAnimeSchedule(anime, images, dayOfWeek);
                        item.IsCollected = animeIdCollcetedSet.Contains(item.AnimeId);
                        list.Add(item);
                    }
                }

                if (animes.Count > 0)
                    await redis.ListAddAsync(key, list);
            }

            return list;
        }

        private async Task<List<AnimeScheduleInfo>> AddOrUpdateSeasonAnimes(int seasonId, int dayOfWeek)
        {
            List<AnimeScheduleInfo> list = [];
            var season = await db.Queryable<Season>().Where(m => m.Id == seasonId).FirstAsync();
            if (null == season) return list;

            var seasonOfYear = GetSeaonOfYear(season.SeasonOfYear ?? "");
            //var year = seasonOfYear == JikanDotNet.Season.Winter ? season.Year + 1 : season.Year;
            var year = season.Year;
            var page = 1;
            while (true)
            {
                var data = await jikan.GetSeasonAsync(year, seasonOfYear, page);
                if (data.Data.Count == 0 || !data.Pagination.HasNextPage) break;

                await db.Ado.BeginTranAsync();
                try
                {
                    foreach (var source in data.Data)
                    {
                        (var anime, var images) = ToSysAnime(source, seasonId);

                        await animeService.AddOrUpdateAnime(anime);
                        await animeService.AddOrUpdateAnimeImages(anime, images);
                        await animeService.AddOrUpdateAnimeGenres(anime, source.Genres);
                        if (dayOfWeek == anime.BroadcastDay)
                        {
                            list.Add(ToAnimeSchedule(anime, images, dayOfWeek));
                        }

                    }
                    await db.Ado.CommitTranAsync();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                    await db.Ado.RollbackTranAsync();
                }

                await Task.Delay(200);
                page++;

                //break;
            }

            return list;
        }

        private static string GetAnimeImageUrl(List<AnimeImage> images)
        {
            var image = images.FirstOrDefault(m => m.ImageType == AnimeImageType.DEFAULT) ?? images.FirstOrDefault();
            if (image == null) return string.Empty;

            return !string.IsNullOrEmpty(image.StorageUrl) ? $"image/anime_cover/{image.AnimeId}" :
                image.RemoteUrl ?? string.Empty;
        }

        private static (Anime, List<AnimeImage>) ToSysAnime(JikanDotNet.Anime source, int seasonId)
        {
            var day = source.Broadcast.Day?.Replace("days", "day") ?? string.Empty;
            //images  genres
            Anime anime = new()
            {
                Name = source.Titles.FirstOrDefault()?.Title ?? string.Empty,
                Alias = string.Empty,
                Description = source.Synopsis,
                SeasonId = seasonId,
                Status = GetAnimeStatus(source.Status),
                AiringDate = source.Aired.From,
                AiriedDate = source.Aired.To,
                BroadcastDay = ConvUtil.AsEnumValue<DayOfWeek>(day),
                BroadcastTime = source.Broadcast.Time,
                EpisodeCount = source.Episodes,
                Scroe = source.Score,
                Rank = source.Rank,
                Rating = source.Rating,
                CreateTime = DateTime.Now,
            };

            List<AnimeImage> images = [];
            const string JPG = "jpg";

            TryAddImage(images, source.Images.JPG.ImageUrl, AnimeImageType.DEFAULT, JPG);
            TryAddImage(images, source.Images.JPG.SmallImageUrl, AnimeImageType.SMALL, JPG);
            TryAddImage(images, source.Images.JPG.MediumImageUrl, AnimeImageType.MEDUIM, JPG);
            TryAddImage(images, source.Images.JPG.LargeImageUrl, AnimeImageType.LARGE, JPG);

            return (anime, images);
        }

        private static AnimeScheduleInfo ToAnimeSchedule(Anime anime, List<AnimeImage> images, int dayOfWeek)
        {
            return new()
            {
                Title = anime.Name ?? string.Empty,
                AnimeId = anime.Id,
                DayOfWeek = dayOfWeek,
                ImageUrl = GetAnimeImageUrl(images),
                Score = anime.Scroe ?? 0
            };
        }

        private static void TryAddImage(List<AnimeImage> list, string url, string imageType, string suffix)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var image = new AnimeImage()
                {
                    ImageType = imageType,
                    Suffix = suffix,
                    RemoteUrl = url,
                };
                list.Add(image);
            }
        }

        private static AnimeStatus GetAnimeStatus(string status)
        {
            //"Finished Airing" "Currently Airing" "Not yet aired"
            return status switch
            {
                "Finished Airing" => AnimeStatus.Archived,
                "Currently Airing" => AnimeStatus.Airing,
                "Not yet aired" => AnimeStatus.Upcoming,
                _ => AnimeStatus.Unknown,
            };
        }

        private static JikanDotNet.Season GetSeaonOfYear(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            if ("1月番" == value) return JikanDotNet.Season.Winter;
            if ("4月番" == value) return JikanDotNet.Season.Spring;
            if ("7月番" == value) return JikanDotNet.Season.Summer;
            if ("10月番" == value) return JikanDotNet.Season.Fall;

            throw new ArgumentException("未知的番剧季度！");
        }

        private static string GetSeasonName(DateTime date)
        {
            return date.Month switch
            {
                1 or 2 or 3 => "1月番",
                4 or 5 or 6 => "4月番",
                7 or 8 or 9 => "7月番",
                _ => "10月番",
            };
        }

        private static List<Season> GetAllSeasons()
        {
            var now = DateTime.Now;
            var sequence = 0;
            var cursor = DateTime.Parse("1980-01-01");
            var seasons = new List<Season>();
            while (true)
            {
                var seaonName = GetSeasonName(cursor);
                var season = new Season()
                {
                    Name = cursor.Year + "年" + seaonName,
                    Year = cursor.Year,
                    SeasonOfYear = seaonName,
                    StartDate = cursor,
                    EndDate = cursor.AddMonths(3).AddSeconds(-1),
                    SeasonNumber = sequence,
                    CreateDate = now,
                };
                seasons.Add(season);
                if (cursor.AddMonths(3) > now) break;
                sequence++;
                cursor = cursor.AddMonths(3);
            }

            return seasons;
        }




    }
}
