using Microsoft.EntityFrameworkCore;
using Ocelot.ConfigurationProvider.EntityFramework.Models;

namespace Ocelot.ConfigurationProvider.EntityFramework
{
    public class OcelotDbContext : DbContext
    {
        public OcelotDbContext(DbContextOptions<OcelotDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReRoute>().ToTable("OcelotReRoutes");
            modelBuilder.Entity<GlobalConfiguration>().ToTable("OcelotGlobalConfigurations");
        }

        public DbSet<GlobalConfiguration> GlobalConfigurations { get; set; }

        public DbSet<OcelotConfiguration> OcelotConfigurations { get; set; }

        public DbSet<ReRoute> ReRoutes { get; set; }
    }
}
