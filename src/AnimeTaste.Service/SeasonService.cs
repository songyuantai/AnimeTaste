using AnimeTaste.Core.Const;
using AnimeTaste.Core.Utils;
using AnimeTaste.Model;
using AnimeTaste.Service.Cache;
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
            var list = await redis.ListGet<Season>(SeasonKey);
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

            //删除缓存
            await redis.KeyDeleteAsync(SeasonKey);

            await redis.ListAdd(SeasonKey, list);

            return list;
        }

        public async Task<List<Anime>> GetOrAddSeasonAnimeList(int seasonId, int dayOfWeek)
        {
            const string DayAnimeList = "dayAnimeList";
            var key = $"{DayAnimeList}:{seasonId}:{dayOfWeek}";
            if (seasonId == 0 || dayOfWeek < 1 || dayOfWeek > 7) return [];

            //var list = await redis.ScanEntityList<Anime>(key);
            var list = await redis.ListGet<Anime>(key);
            if (list.Count == 0)
            {
                list = await db.Queryable<Anime>()
                    .Where(m => m.SeasonId == seasonId && m.BroadcastDay == dayOfWeek)
                    .ToListAsync();

                if (list.Count == 0)
                {
                    var animes = await AddOrUpdateSeasonAnimes(seasonId);
                    foreach (var g in animes.GroupBy(m => m.BroadcastDay))
                    {
                        var groupKey = $"{DayAnimeList}:{seasonId}:{g.Key}";
                        var gorupList = g.ToList();
                        await redis.ListReplace(groupKey, gorupList);
                        if (g.Key == dayOfWeek)
                        {
                            list = gorupList;
                        }
                    }
                }
                else
                {
                    await redis.ListAdd(key, list);
                }
            }

            return list;
        }

        private async Task<List<Anime>> AddOrUpdateSeasonAnimes(int seasonId)
        {
            List<Anime> list = [];
            var season = await db.Queryable<Season>().Where(m => m.Id == seasonId).FirstAsync();
            if (null == season) return list;

            var seasonOfYear = GetSeaonOfYear(season.SeasonOfYear ?? "");
            var year = seasonOfYear == JikanDotNet.Season.Winter ? season.Year - 1 : season.Year;
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
                        list.Add(anime);

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
            }

            return list;
        }

        private static (Anime, List<AnimeImage>) ToSysAnime(JikanDotNet.Anime source, int seasonId)
        {
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
                BroadcastDay = ConvUtil.AsEnumValue<DayOfWeek>(source.Broadcast.Day),
                BroadcastTime = source.Broadcast.Time,
                EpisodeCount = source.Episodes,
                Scroe = source.Score,
                Rank = source.Rank,
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
