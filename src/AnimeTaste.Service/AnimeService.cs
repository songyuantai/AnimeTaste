using AnimeTaste.Model;
using AnimeTaste.Service.Cache;
using AnimeTaste.ViewModel;
using SqlSugar;

namespace AnimeTaste.Service
{
    /// <summary>
    /// 番剧服务
    /// </summary>
    /// <param name="db"></param>
    public class AnimeService(ISqlSugarClient db, RedisService redis)
    {
        /// <summary>
        /// 添加或者更新番剧信息
        /// </summary>
        /// <param name="animes"></param>
        /// <returns></returns>
        public async Task AddOrUpdateAnime(params List<Anime> animes)
        {
            foreach (var anime in animes)
            {
                if (!await db.Queryable<Anime>().Where(m => m.Name == anime.Name).AnyAsync())
                {
                    await db.Insertable(anime).ExecuteCommandIdentityIntoEntityAsync();
                }
                else
                {
                    await db.Updateable(anime)
                        .IgnoreColumns(nameof(Anime.Id))
                        .WhereColumns(nameof(Anime.Name))
                        .ExecuteCommandAsync();
                }
            }
        }

        /// <summary>
        /// 添加或者更新番剧图片
        /// </summary>
        /// <param name="anime"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task AddOrUpdateAnimeImages(Anime anime, params List<AnimeImage> images)
        {
            foreach (var image in images)
            {
                var entity = await db.Queryable<AnimeImage>().Where(m => m.AnimeId == anime.Id && m.ImageType == image.ImageType).FirstAsync();
                if (null == entity)
                {
                    image.AnimeId = anime.Id;
                    await db.Insertable(image).ExecuteCommandIdentityIntoEntityAsync();
                }
                else
                {
                    entity.RemoteUrl = image.RemoteUrl;
                    entity.StorageUrl = image.StorageUrl;
                    entity.StorageType = image.StorageType;

                    await db.Updateable(entity)
                        .ExecuteCommandAsync();
                }
            }
        }

        /// <summary>
        /// 添加或者更新番剧类别
        /// </summary>
        /// <param name="anime"></param>
        /// <param name="genres"></param>
        /// <returns></returns>
        public async Task AddOrUpdateAnimeGenres(Anime anime, params ICollection<JikanDotNet.MalUrl> genres)
        {
            foreach (var genre in genres)
            {
                var entity = await db.Queryable<Genre>()
                    .Where(m => m.Name == genre.Name)
                    .FirstAsync();

                if (null == entity)
                {
                    entity = new()
                    {
                        Name = genre.Name,
                        Alias = string.Empty,
                        Url = genre.Url,
                        CreateDate = DateTime.Now,
                    };

                    await db.Insertable(entity).ExecuteCommandIdentityIntoEntityAsync();

                    await db.Insertable(new AnimeGenre { AnimeId = anime.Id, GenresId = entity.Id })
                        .ExecuteCommandAsync();
                }
                else
                {
                    if (!await db.Queryable<AnimeGenre>()
                        .Where(m => m.AnimeId == anime.Id && m.GenresId == entity.Id)
                        .AnyAsync())
                    {
                        await db.Insertable(new AnimeGenre { AnimeId = anime.Id, GenresId = entity.Id })
                        .ExecuteCommandAsync();
                    }
                }
            }
        }


        public async Task AnimeCollectToggle(int animeId, int seasonId, int dayOfWeek, bool isCollect)
        {
            if (animeId <= 0) return;

            var isCollected = await db.Queryable<AnimeCollection>().Where(m => m.AnimeId == animeId).AnyAsync();
            if (isCollected && !isCollect)
            {
                //取消收藏
                await db.Deleteable<AnimeCollection>().Where(m => m.AnimeId == animeId).ExecuteCommandAsync();
            }
            else if (!isCollected && isCollect)
            {
                //收藏
                var entity = new AnimeCollection
                {
                    AnimeId = animeId,
                    CollectDate = DateTime.Now,
                };
                await db.Insertable(entity).ExecuteCommandAsync();
            }

            var key = $"{SeasonService.AnimeScheduleList}:{seasonId}:{dayOfWeek}";
            var animeScheduleInfoList = await redis.ListGetAsync<AnimeScheduleInfo>(key);
            var animeScheduleInfo = animeScheduleInfoList.FirstOrDefault(m => m.AnimeId == animeId);
            if (animeScheduleInfo != null)
            {
                animeScheduleInfo.IsCollected = isCollect;
                await redis.ListReplaceAsync(key, animeScheduleInfoList);
            }
        }
    }
}
