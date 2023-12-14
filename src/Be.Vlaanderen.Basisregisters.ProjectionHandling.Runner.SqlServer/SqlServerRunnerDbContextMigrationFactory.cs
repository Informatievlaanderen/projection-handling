namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.SqlServer
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MigrationExtensions;

    public abstract class SqlServerRunnerDbContextMigrationFactory<TContext> : RunnerDbContextMigrationFactoryBase<TContext>, IDesignTimeDbContextFactory<TContext>, IRunnerDbContextMigratorFactory
        where TContext : RunnerDbContext<TContext>
    {
        protected SqlServerRunnerDbContextMigrationFactory(
            string connectionStringName,
            MigrationHistoryConfiguration migrationHistoryConfiguration)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException(nameof(connectionStringName));

            _connectionStringName = connectionStringName;

            _migrationHistoryConfiguration = migrationHistoryConfiguration ?? throw new ArgumentNullException(nameof(migrationHistoryConfiguration));
        }

        private readonly string _connectionStringName;
        private readonly MigrationHistoryConfiguration _migrationHistoryConfiguration;

        protected virtual void ConfigureSqlServerOptions(SqlServerDbContextOptionsBuilder sqlServerOptions) { }


        public IRunnerDbContextMigrator CreateMigrator(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var contextOptions = CreateOptionsBuilder(configuration, loggerFactory).Options;

            return new SqlServerRunnerDbContextMigrator<TContext>(() => CreateContext(contextOptions), loggerFactory);
        }

        protected override DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfiguration configuration)
        {
            var connectionString = configuration?.GetConnectionString(_connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Could not find a connection string with name '{connectionString}'");

            var optionsBuilder = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();

                        sqlServerOptions.MigrationsHistoryTable(
                            _migrationHistoryConfiguration.Table,
                            _migrationHistoryConfiguration.Schema);

                        ConfigureSqlServerOptions(sqlServerOptions);
                    }
                )
                .UseExtendedSqlServerMigrations();

            ConfigureOptionsBuilder(optionsBuilder);

            return optionsBuilder;
        }

        private DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            return CreateOptionsBuilder(configuration)
                .UseLoggerFactory(loggerFactory);
        }
    }
}
