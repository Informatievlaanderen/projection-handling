namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Microsoft
{
    using System;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
    using Runner.MigrationExtensions;
    using DependencyInjection;
    using global::Microsoft.Data.SqlClient;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Logging;

    public class LastChangedListModule : IServiceCollectionModule
    {
        private readonly string _connectionString;
        private readonly string _datadogServiceName;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public LastChangedListModule(
            string connectionString,
            string datadogServiceName,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _datadogServiceName = datadogServiceName;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        private static void RunOnSqlServer(
            string datadogServiceName,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            string backofficeProjectionsConnectionString)
        {
            services
                .AddScoped(s =>
                    new TraceDbConnection<LastChangedListContext>(
                        new SqlConnection(backofficeProjectionsConnectionString),
                        datadogServiceName))
                .AddDbContext<LastChangedListContext>((provider, options) => options
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(
                        provider.GetRequiredService<TraceDbConnection<LastChangedListContext>>(),
                        sqlServerOptions =>
                        {
                            sqlServerOptions.EnableRetryOnFailure();
                            sqlServerOptions.MigrationsHistoryTable(LastChangedListContext.MigrationsHistoryTable, LastChangedListContext.Schema);
                        })
                    .UseExtendedSqlServerMigrations());
        }

        private static void RunInMemoryDb(
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            ILogger<LastChangedListModule> logger)
        {
            services
                .AddDbContext<LastChangedListContext>(options => options
                    .UseLoggerFactory(loggerFactory)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()));

            logger.LogWarning("Running InMemory for {Context}!", nameof(LastChangedListContext));
        }

        public void Load(IServiceCollection services)
        {
            var logger = _loggerFactory.CreateLogger<LastChangedListModule>();

            var hasConnectionString = !string.IsNullOrWhiteSpace(_connectionString);
            if (hasConnectionString)
            {
                RunOnSqlServer(_datadogServiceName, services, _loggerFactory, _connectionString);
            }
            else
            {
                RunInMemoryDb(services, _loggerFactory, logger);
            }

            logger.LogInformation(
                "Added {Context} to services:" +
                Environment.NewLine +
                "\tSchema: {Schema}" +
                Environment.NewLine +
                "\tTableName: {TableName}",
                nameof(LastChangedListContext), LastChangedListContext.Schema, LastChangedListContext.MigrationsHistoryTable);
        }
    }
}
