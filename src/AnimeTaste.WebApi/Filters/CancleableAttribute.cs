using Microsoft.AspNetCore.Mvc.Filters;

namespace AnimeTaste.WebApi.Filters
{
    /// <summary>
    /// 不太行，不能真的取消
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CancelableAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var cancellationToken = context.HttpContext.RequestAborted;

            var actionTask = next();

            var completedTask = await Task.WhenAny(actionTask, Task.Delay(Timeout.Infinite, cancellationToken));

            if (completedTask != actionTask)
            {
                // 取消时抛出异常，终止请求管道
                throw new OperationCanceledException(cancellationToken);
            }
            await actionTask;
        }
    }
}
