using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                .ConfigureAppConfiguration((hostBuilder, configBuilder) =>
                {
                    configBuilder.AddJsonFile("abc.json", optional: true, reloadOnChange: false);
                })
                .UseRedisConfiguration(action =>
                {
                    action.CachePrefix = "AspNetCorePlayground";
                    action.RedisServers = new[]
                    {
                        new RedisServerConfiguration("127.0.0.1", 6379),
                    };
                    action.DefaultDatabase = 2;
                })
                .UseStartup<Startup>();
    }
}
