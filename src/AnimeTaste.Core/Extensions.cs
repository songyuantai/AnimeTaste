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

        /// <summary>
        /// 异步循环（无序）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static Task LoopAsync<T>(this IEnumerable<T> list, Func<T, Task> function)
        {
            return Task.WhenAll(list.Select(function));
        }

        /// <summary>
        /// 异步循环并返回结果（无序）
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="list"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<TOut>> LoopAsyncResult<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, Task<TOut>> function)
        {
            var loopResult = await Task.WhenAll(list.Select(function));

            return loopResult.ToList().AsEnumerable();
        }

        /// <summary>
        /// 选择并筛选非空的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<V> SelectNotNull<T, V>(this IEnumerable<T> source, Func<T, V?> selector)
        {
            foreach (var item in source)
            {
                var v = selector(item);
                if (v is not null)
                {
                    yield return v; // 编译器自动推断v为非空
                }
            }
        }

        /// <summary>
        /// 获取指定步长的嵌套list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetNestList<T>(this IEnumerable<T> source, int step)
        {
            return source.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / step)
                            .Select(g => g.Select(x => x.Value).ToList())
                            .ToList();
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(Result<>));
            services.AddSingleton(typeof(Ai));
        }

    }
}
