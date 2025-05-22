using AnimeTaste.Core;
using Dm.util;
using StackExchange.Redis;

namespace AnimeTaste.Service.Cache
{
    public class RedisService(ConnectionMultiplexer conn)
    {
        private IDatabase db = conn.GetDatabase();

        /// <summary>
        /// 列表插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> ListAddAsync<T>(string key, params List<T> list) where T : class
        {
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
        public async Task<List<T>> ListGetAsync<T>(string key, int start = 0, int end = -1) where T : class
        {
            var list = await db.ListRangeAsync(key, start, end);
            return [.. list.SelectNotNull(x => x.ToString().ToObject<T>())];
        }

        /// <summary>
        /// 列表替换（先删除）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> ListReplaceAsync<T>(string key, List<T> list, CancellationToken token) where T : class
        {
            if (await db.KeyExistsAsync(key))
            {
                if (!await db.KeyDeleteAsync(key))
                {
                    return false;
                }
            }
            return await ListAddAsync(key, list);
        }

        /// <summary>
        /// key删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> KeyDeleteAsync(string key)
        {
            var db = conn.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 实体搜索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="match"></param>
        /// <param name="rangeCount"></param>
        /// <returns></returns>
        public async Task<List<T>> ScanEntityList<T>(string match, int rangeCount = 10) where T : class
        {
            //var keys = await ScanKeys(match, rangeCount);
            //var data = await keys.GetNestList(rangeCount).LoopAsyncResult((t) => db.StringGetAsync([.. t]));
            //return [.. data.SelectMany(x => x).Select(m => m.ToString().ToObject<T>()).PickNotNull(m => m)];
            List<T> list = [];
            await foreach (var keys in ScanKeysAsyncEnumerable(match, rangeCount))
            {
                var result = await db.StringGetAsync(keys);
                list.AddRange(result.Select(m => m.toString().ToObject<T>()).SelectNotNull(m => m));
            }
            return list;
        }

        /// <summary>
        /// key扫描
        /// </summary>
        /// <param name="match"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<RedisKey>> ScanKeys(string match, int count)
        {
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

        public async IAsyncEnumerable<RedisKey[]> ScanKeysAsyncEnumerable(string match, int count)
        {
            List<RedisKey> keys = [];

            long cursor = 0;
            do
            {
                var scanResult = await db.ExecuteAsync("SCAN", cursor, "COUNT", count, "MATCH", match);
                cursor = (long)scanResult[0];
#nullable disable
                yield return ((RedisResult[])scanResult[1]).Select(m => (RedisKey)(string)m).ToArray();
#nullable restore
            } while (cursor != 0);

        }

        /// <summary>
        /// key扫描
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

        /// <summary>
        /// key扫描（包含所有server）
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="match"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
