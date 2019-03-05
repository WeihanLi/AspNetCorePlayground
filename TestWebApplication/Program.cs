using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using WeihanLi.Configuration.EntityFramework;

namespace TestWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    var configuration = configBuilder.Build();
                    configBuilder.AddEntityFramework(config => config.UseInMemoryDatabase("Configurations"));
                })
                .UseStartup<Startup>();
    }
}
