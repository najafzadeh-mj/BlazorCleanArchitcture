//using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetcodeHub.Packages.Components.Toast;
using WebUI;
using Application.DependencyInjection;
using NetcodeHub.Packages.Components.DataGrid;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddApplicationService();
builder.Services.AddVirtualizationService();
//builder.Services.AddScoped<ToastService>();

await builder.Build().RunAsync();
