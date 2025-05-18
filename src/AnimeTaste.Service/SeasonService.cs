using AnimeTaste.Model;
using AnimeTaste.Service.Cache;
using SqlSugar;

namespace AnimeTaste.Service
{
    public class SeasonService(ISqlSugarClient db, RedisService redis)
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
