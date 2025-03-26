using Microsoft.JSInterop;

namespace AnimeTaste.Client
{
    public class JsService(IJSRuntime js)
    {
        public async Task SetLocalStorageAsync(string key, string value)
        {
            await js.InvokeAsync<string>("localStorage.setItem", key, value);
        }

        public async Task<string> GetLocalStorageAsync(string key)
        {
            return await js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public async Task RemoveLocalStorageAsync(string key)
        {
            await js.InvokeAsync<string>("localStorage.removeItem", key);
        }
    }
}
