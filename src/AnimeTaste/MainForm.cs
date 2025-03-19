using AnimeTaste.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AnimeTaste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
            services.AddAntDesign();

            mainBlazorWebView.HostPage = "wwwroot\\index.html";

            services.AddAuthorizationCore();
            services.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
            mainBlazorWebView.Services = services.BuildServiceProvider();


            mainBlazorWebView.UrlLoading += (sender, urlLoadingEventArgs) =>
            {
                if (urlLoadingEventArgs.Url.Host != "0.0.0.0")
                {
                    urlLoadingEventArgs.UrlLoadingStrategy =
                        UrlLoadingStrategy.OpenInWebView;
                }
            };
            mainBlazorWebView.RootComponents.Add<App>("#app");

        }
    }
}
