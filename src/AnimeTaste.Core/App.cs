using Microsoft.Extensions.DependencyInjection;

namespace AnimeTaste.Core
{
    public static class App
    {
        private static IServiceProvider? _provider;

        public static void SetProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static T? Resolve<T>() => _provider!.GetService<T>();
    }
}
