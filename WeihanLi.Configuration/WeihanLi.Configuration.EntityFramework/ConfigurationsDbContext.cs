using Microsoft.EntityFrameworkCore;

namespace WeihanLi.Configuration.EntityFramework
{
    public class ConfigurationsDbContext : DbContext
    {
        public DbSet<Configuration> Configurations { get; set; }

        public ConfigurationsDbContext(DbContextOptions<ConfigurationsDbContext> options) : base(options)
        {
        }
    }
}
