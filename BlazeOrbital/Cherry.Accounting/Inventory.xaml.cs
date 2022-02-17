using Cherry.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Controls;

namespace Cherry.Accounting
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl
    {
        public Inventory()
        {
            var services = new ServiceCollection();
            services.AddBlazorWebView();
            services.AddDataClient((services, options) =>
            {
                options.BaseUri = WpfAppAccessTokenProvider.Instance.BackendUrl;
                options.MessageHandler = WpfAppAccessTokenProvider.Instance.CreateMessageHandler(services);
            });

            // Sets up EF Core with Sqlite
            services.AddDataDbContext();

            Resources.Add("services", services.BuildServiceProvider());

            InitializeComponent();
        }
    }
}
