using AnimeTaste.Auth;
using AnimeTaste.Client;
using AnimeTaste.Core;
using AnimeTaste.Core.Const;
using AnimeTaste.Core.Utils;
using AnimeTaste.Service;
using AnimeTaste.WebApi;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace AnimeTaste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            services.AddOptions();
            services.AddWindowsFormsBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
            services.AddAntDesign();

            mainBlazorWebView.HostPage = "wwwroot\\index.html";

            services.AddBlazoredLocalStorage();

            services.AddAuthorizationCore(op =>
            {
                Policy.GetPolicyList().ForEach(policy => op.AddPolicy(policy, m => m.RequireClaim(policy, "true")));
            });
            services.AddScoped<JsService>();
            services.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
            services.AddCascadingAuthenticationState();
            services.AddSugarSql("server=localhost;userid=root;password=root;database=anime;AllowLoadLocalInfile=true");
            services.AddSysServices();
            services.AddCoreServices();
            services.AddScoped<ApiClient>();

            mainBlazorWebView.Services = services.BuildServiceProvider();
            Core.App.SetProvider(mainBlazorWebView.Services);

            var data = Core.App.Resolve<Ai>()!.TranslateToChinese("Hello World!");
            Debug.WriteLine(data);

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            BeginInvoke(() =>
            {
                //var maintain = mainBlazorWebView.Services.GetService<Service.DbMaintain.DbMaintainService>();
                // maintain?.DropTables();
                //maintain?.InitTables();
                //maintain?.SeedData();
            });

        }

        private void mainBlazorWebView_Click(object sender, EventArgs e)
        {

        }
    }
}
