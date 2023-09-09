using ConveyorBlazorServerNet7;
using ConveyorBlazorServerNet7.Data;
using ConveyorBlazorServerNet7.Hubs;
using ConveyorLibWeb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;
using ScriptingLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

//builder.Services.AddResponseCompression(opts =>
//{
//    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//          new[] { "application/octet-stream" });
//});

var app = builder.Build();

//app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();
app.MapHub<ConveyorHub>("/conveyorhub");


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

UIHelpers.Instance = new UIHelpersInstanceWebCanvas();

AppContent.Init();
//AppContent.AutoRoot.AddLine(((0, 0), (200, 200)));
//AppContent.AutoRoot.AddLine(((100, 100), (0, 200)));

//AppContent.AutoRoot.AddCircleCenterRadius(((100, 100), 50));

app.Run();
