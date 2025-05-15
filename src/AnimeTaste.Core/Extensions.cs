using AnimeTaste.Core.Model;
using AnimeTaste.Core.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnimeTaste.Core
{
    public static class Extensions
    {
        public static Result<T> Ok<T>(this Result<T> result, string? message = null, T? data = default)
        {
            result.IsSuccess = true;
            result.Message = message;
            result.Data = data;
            return result;
        }

        public static Result<T> Fail<T>(this Result<T> result, string? message = null, T? data = default)
        {
            result.IsSuccess = false;
            result.Message = message;
            result.Data = data;
            return result;
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(Result<>));
            services.AddSingleton(typeof(Ai));
        }

    }
}
