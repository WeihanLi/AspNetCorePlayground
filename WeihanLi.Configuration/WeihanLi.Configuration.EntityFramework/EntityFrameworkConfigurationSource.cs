using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.EntityFramework
{
    internal class EntityFrameworkConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder<ConfigurationsDbContext>> _action;

        public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder<ConfigurationsDbContext>> action)
        {
            _action = action;
        }

        private readonly DbContextOptionsBuilder<ConfigurationsDbContext> DbContextOptionsBuilder = new DbContextOptionsBuilder<ConfigurationsDbContext>();

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            _action.Invoke(DbContextOptionsBuilder);
            return new EntityFrameworkConfigurationProvider(DbContextOptionsBuilder.Options);
        }
    }
}
