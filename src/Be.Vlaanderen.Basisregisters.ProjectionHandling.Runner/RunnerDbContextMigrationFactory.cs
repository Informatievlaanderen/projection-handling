namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public abstract class RunnerDbContextMigrationFactory<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : RunnerDbContext<TContext>
    {
        protected RunnerDbContextMigrationFactory(
            string connectionStringName,
            MigrationHistoryConfiguration migrationHistoryConfiguration)
        {
            if(string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException(nameof(connectionStringName));
            _connectionStringName = connectionStringName;

            _migrationHistoryConfiguration = migrationHistoryConfiguration ?? throw new ArgumentNullException(nameof(migrationHistoryConfiguration));
        }

        private readonly string _connectionStringName;
        private readonly MigrationHistoryConfiguration _migrationHistoryConfiguration;
        protected abstract TContext CreateContext(DbContextOptions<TContext> migrationContextOptions);

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

        public IRunnerDbContextMigrator CreateMigrator(IConfigurationRoot configuration, ILoggerFactory loggerFactory)
        {
            var contextOptions = CreateOptionsBuilder(configuration, loggerFactory).Options;

            return new RunnerDbContextMigrator<TContext>(() => CreateContext(contextOptions), loggerFactory);
        }

        private DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfigurationRoot configuration)
        {
            var connectionString = configuration?.GetConnectionString(_connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Could not find a connection string with name '{connectionString}'");
            
            return new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.MigrationsHistoryTable(
                            _migrationHistoryConfiguration.Table,
                            _migrationHistoryConfiguration.Schema);
                    }
                );
        }

        private DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfigurationRoot configuration, ILoggerFactory loggerFactory)
        {
            if (null == loggerFactory)
                throw new ArgumentNullException(nameof(loggerFactory));

            return CreateOptionsBuilder(configuration)
                .UseLoggerFactory(loggerFactory);
        }

        public class MigrationHistoryConfiguration
        {
            public string Schema { get; set; }
            public string Table { get; set; }
        }
    }
}
