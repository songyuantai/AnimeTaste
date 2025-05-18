using AnimeTaste.Core;
using StackExchange.Redis;

namespace AnimeTaste.Service.Cache
{
    public class RedisService(ConnectionMultiplexer conn)
    {
        /// <summary>
        /// 列表插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> ListAdd<T>(string key, params List<T> list) where T : class
        {
            var db = conn.GetDatabase();
            foreach (var item in list)
            {
                var value = item.ToJson();
                await db.ListRightPushAsync(key, value);
            }

            return true;
        }

        /// <summary>
        /// 列表获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListGet<T>(string key, int start = 0, int end = -1) where T : class
        {
            var db = conn.GetDatabase();
            var list = await db.ListRangeAsync(key, start, end);
            return [.. list.PickNotNull(x => x.ToString().ToObject<T>())];
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            var db = conn.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<RedisKey>> ScanKeys(string match, int count)
        {
            var db = conn.GetDatabase();

            List<RedisKey> keys = [];

            long cursor = 0;
            do
            {
                var scanResult = await db.ExecuteAsync("SCAN", cursor, "COUNT", count, "MATCH", match);
                cursor = (long)scanResult[0];

#nullable disable
                foreach (var redisKey in (RedisResult[])scanResult[1])
                {
                    keys.Add((string)redisKey);
                }
#nullable restore
            } while (cursor != 0);

            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="match"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<RedisKey>> ScanKeys(IServer server, string match, int count)
        {
            List<RedisKey> keys = new List<RedisKey>();

            long cursor = 0;
            do
            {
                var scanResult = await server.ExecuteAsync("SCAN", cursor, "COUNT", count, "MATCH", match);
                cursor = (long)scanResult[0];

#nullable disable
                foreach (var redisKey in (RedisResult[])scanResult[1])
                {
                    keys.Add((string)redisKey);
                }
#nullable restore
            } while (cursor != 0);

            return keys;
        }

        public async Task<List<RedisKey>> ScanKeys(ConnectionMultiplexer conn, string match, int count)
        {
            var ipList = conn.GetEndPoints().Select(m => m.ToString()).ToList();
            var result = new List<RedisKey>();
            foreach (var ip in ipList)
            {
                if (null != ip)
                {
                    var server = conn.GetServer(ip);
                    if (null == server) break;
                    var list = await ScanKeys(server, match, count);
                    result.AddRange(list);
                }
            }
            return result;
        }

    }
}
