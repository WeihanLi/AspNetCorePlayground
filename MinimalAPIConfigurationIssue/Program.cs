using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"builder.Configuration hash code: {builder.Configuration.GetHashCode()}");
builder.Services.Configure<SettingsOption>(builder.Configuration);
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Test");
app.Map(new PathString("/redeploy"), appBuilder => appBuilder.Run(context =>
{
    var config = context.RequestServices.GetRequiredService<IConfiguration>();
    logger.LogInformation($"Configuration in redeploy hash code: {config.GetHashCode()}");
    if (config is IConfigurationRoot configuration)
    {
        var currentSlot = configuration["Setting1"];
        configuration["Setting1"] = "Slot2" != currentSlot ? "Slot2" : "Slot1";
        configuration.Reload();
        return context.Response.WriteAsync("Success");
    }
    logger.LogWarning($"configuration is not configuration root, configurationType: {config.GetType().FullName}");
    return Task.CompletedTask;
}));
app.MapFallback(context =>
{
    var option = context.RequestServices.GetRequiredService<IOptionsMonitor<SettingsOption>>();
    return context.Response.WriteAsJsonAsync(new{ option.CurrentValue.Setting1, Time= DateTime.Now });
});
app.Run();

public class SettingsOption
{
    public string Setting1 { get; set; }
}