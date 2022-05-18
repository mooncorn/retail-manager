using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portal;
using Portal.Authentication;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services
    .AddSingleton<IAPIHelper, APIHelper>()
    .AddSingleton<ILoggedInUserModel, LoggedInUserModel>();

builder.Services
    .AddTransient<IProductEndpoint, ProductEndpoint>()
    .AddTransient<IUserEndpoint, UserEndpoint>()
    .AddTransient<ISaleEndpoint, SaleEndpoint>();

//_container
//    .PerRequest<IProductEndpoint, ProductEndpoint>()
//    .PerRequest<ISaleEndpoint, SaleEndpoint>()
//    .PerRequest<IUserEndpoint, UserEndpoint>();

//_container
//    .Singleton<IWindowManager, WindowManager>()
//    .Singleton<IEventAggregator, EventAggregator>()
//    .Singleton<ILoggedInUserModel, LoggedInUserModel>()
//    .Singleton<IAPIHelper, APIHelper>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();



