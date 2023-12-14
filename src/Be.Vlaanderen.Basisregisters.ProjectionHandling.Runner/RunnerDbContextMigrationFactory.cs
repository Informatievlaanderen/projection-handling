namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public interface IRunnerDbContextMigratorFactory
    {
        IRunnerDbContextMigrator CreateMigrator(IConfiguration configuration, ILoggerFactory loggerFactory);
    }

    public abstract class RunnerDbContextMigrationFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : RunnerDbContext<TContext>
    {
        protected abstract TContext CreateContext(DbContextOptions<TContext> migrationContextOptions);
        protected virtual void ConfigureOptionsBuilder(DbContextOptionsBuilder<TContext> optionsBuilder) { }

        protected abstract DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfiguration configuration);

        public TContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var contextOptions = CreateOptionsBuilder(configuration).Options;

            return CreateContext(contextOptions);
        }

        public class MigrationHistoryConfiguration
        {
            public string Schema { get; set; }
            public string Table { get; set; }
        }
    }
}
