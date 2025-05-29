using AnimeTaste.Service.Cache;
using AnimeTaste.Service.DbMaintain;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Redis;

namespace AnimeTaste.Service
{
    public static class Extensions
    {
        public static void AddSugarSql(this IServiceCollection services, string? connectString)
        {
            if (string.IsNullOrWhiteSpace(connectString)) throw new ArgumentNullException(nameof(connectString));

            services.AddScoped<ISqlSugarClient>(s =>
            {
                SqlSugarClient sqlSugarClient = new(new ConnectionConfig()
                {
                    DbType = DbType.MySql,
                    ConnectionString = connectString,
                    IsAutoCloseConnection = true,
                });
                return sqlSugarClient;
            });
        }

        public static void AddSysServices(this IServiceCollection services)
        {
            services.AddScoped<DbMaintainService>();
            services.AddScoped<UserService>();
            services.AddScoped<RedisService>();
            services.AddScoped<SeasonService>();
            services.AddScoped<AnimeService>();
        }

        public static string ToJson<T>(this T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public static T? ToObject<T>(this string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static T? ToObject<T>(this HashEntry[]? entries) where T : class
        {
            T? result = default;

            if (entries == null || entries.Length == 0)
            {
                return result;
            }

            try
            {
                result = Activator.CreateInstance<T>();
                typeof(T).GetProperties().ToList().ForEach(p =>
                {
                    foreach (var entry in entries)
                    {
                        if (entry.Name == p.Name)
                        {
                            if (!entry.Value.IsNull)
                            {
                                var value = entry.Value.ToString();
                                p.SetValue(result, Convert.ChangeType(value, p.PropertyType));
                            }
                            break;
                        }

                    }
                });
            }
            catch (Exception)
            {

                throw;
            }

            return result;


        }


    }
}
