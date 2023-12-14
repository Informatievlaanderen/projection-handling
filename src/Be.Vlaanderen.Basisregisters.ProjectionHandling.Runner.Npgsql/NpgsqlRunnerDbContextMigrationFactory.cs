namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Npgsql
{
    using System;
    using global::Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public abstract class NpgsqlRunnerDbContextMigrationFactory<TContext> : RunnerDbContextMigrationFactoryBase<TContext>, IDesignTimeDbContextFactory<TContext>, IRunnerDbContextMigratorFactory
        where TContext : RunnerDbContext<TContext>
    {
        protected NpgsqlRunnerDbContextMigrationFactory(
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

        protected virtual void ConfigureSqlServerOptions(NpgsqlDbContextOptionsBuilder serverOptions) { }


        public IRunnerDbContextMigrator CreateMigrator(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var contextOptions = CreateOptionsBuilder(configuration, loggerFactory).Options;

            return new NpgsqlRunnerDbContextMigrator<TContext>(() => CreateContext(contextOptions), loggerFactory);
        }

        protected override DbContextOptionsBuilder<TContext> CreateOptionsBuilder(IConfiguration configuration)
        {
            var connectionString = configuration?.GetConnectionString(_connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Could not find a connection string with name '{connectionString}'");

            var optionsBuilder = new DbContextOptionsBuilder<TContext>()
                .UseNpgsql(
                    options =>
                    {
                        options.EnableRetryOnFailure();

                        options.MigrationsHistoryTable(
                            _migrationHistoryConfiguration.Table,
                            _migrationHistoryConfiguration.Schema);

                        ConfigureSqlServerOptions(options);
                    }
                );

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
