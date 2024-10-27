using ChatApp.Client.Services.Abstract;
using ChatApp.Client.Services.Concrete;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddMudServices();
await builder.Build().RunAsync();
