using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cherry.ManufacturingHub.Data;
using Cherry.ManufacturingHub.Layout;
using Cherry.ManufacturingHub.Pages.Inventory;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
var backendOrigin = builder.Configuration["BackendOrigin"]!;
builder.RootComponents.RegisterAsCustomElement<App>("blazor-app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services
    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Cherry.CentralServerAPI"))
    .AddHttpClient("Cherry.CentralServerAPI", client => client.BaseAddress = new Uri(backendOrigin))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// gRPC-Web client with auth
builder.Services.AddManufacturingDataClient((services, options) =>
{
    var authEnabledHandler = services.GetRequiredService<AuthorizationMessageHandler>();
    authEnabledHandler.ConfigureHandler(new[] { backendOrigin });
    authEnabledHandler.InnerHandler = new HttpClientHandler();

    options.BaseUri = backendOrigin;
    //options.MessageHandler = authEnabledHandler; // with auth // TODO:
    options.MessageHandler = authEnabledHandler.InnerHandler; // No auth
});

// Supplies an IAuthorizationStateProvider service that lets other components know about auth state
// This one gets that state by asking the OpenID Connect client. Also we cache the state for offline use.
builder.Services.AddApiAuthorization(c => c.ProviderOptions.ConfigurationEndpoint = $"{backendOrigin}/_configuration/Cherry.ManufacturingHub");
builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, OfflineAccountClaimsPrincipalFactory>();

// Sets up EF Core with Sqlite
builder.Services.AddManufacturingDataDbContext();

// Declare a custom element for the Mission Control app
builder.RootComponents.RegisterAsCustomElement<Inventory>("inventory-grid");

await builder.Build().RunAsync();
