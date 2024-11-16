using ChatApp;
using ChatApp.Client.Pages;
using ChatApp.Client.Services.Abstract;
using ChatApp.Client.Services.Concrete;
using ChatApp.Components;
using ChatApp.Hubs;
using ChatApp.Services.Abstract;
using ChatApp.Services.Concrete;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddScoped<IChatRepo, ChatRepo>();
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7121/") });
builder.Services.AddScoped<IChatService, ChatService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ChatPage).Assembly);
app.MapHub<ChatHub>("/chathub");
app.Run();
