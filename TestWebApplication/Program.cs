using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WeihanLi.Configuration.Redis;
using WeihanLi.Redis;

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
                .ConfigureServices((context, services) => services.AddRedisConfig(options =>
                {
                }))
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    //var configuration = configBuilder.Build();
                    //configBuilder.AddEntityFramework(config => config.UseInMemoryDatabase("Configurations"));
                    configBuilder.AddRedis(action =>
                    {
                    });
                })
                .UseStartup<Startup>();
    }
}
