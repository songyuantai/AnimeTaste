using AnimeTaste.Service.DbMaintain;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace AnimeTaste.Service
{
    public static class Extensions
    {
        public static void AddSugarSql(this IServiceCollection services, string connectString)
        {
            //注册上下文：AOP里面可以获取IOC对象，如果有现成框架比如Furion可以不写这一行
            //services.AddHttpContextAccessor();
            //注册SqlSugar用AddScoped
            services.AddScoped<ISqlSugarClient>(s =>
            {
                //Scoped用SqlSugarClient 
                SqlSugarClient sqlSugar = new(new ConnectionConfig()
                {
                    DbType = DbType.MySql,
                    ConnectionString = connectString,
                    IsAutoCloseConnection = true,
                });
                return sqlSugar;
            });
        }

        public static void AddSysServices(this IServiceCollection services)
        {
            services.AddScoped<DbMaintainService>();
            services.AddScoped<UserService>();
        }


    }
}
