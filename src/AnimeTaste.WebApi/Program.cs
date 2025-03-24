using AnimeTaste.Core;
using AnimeTaste.Service;
using AnimeTaste.WebApi.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AnimeTaste.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? "")), //SecurityKey
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(60),
                    RequireExpirationTime = true,
                };
            });

            builder.Services.AddSugarSql("server=localhost;userid=root;password=root;database=anime;AllowLoadLocalInfile=true");



            builder.Services.AddCoreServices();

            builder.Services.AddSysServices();

            builder.Services.AddSingleton<JwtHelper>();

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            App.SetProvider(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
