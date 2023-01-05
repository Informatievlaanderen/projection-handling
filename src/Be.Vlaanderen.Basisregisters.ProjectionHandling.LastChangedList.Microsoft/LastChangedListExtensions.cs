namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Microsoft
{
    using System;
    using DataDog.Tracing.Sql.EntityFrameworkCore;
    using global::Microsoft.Data.SqlClient;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Logging;
    using Runner.Microsoft.MigrationExtensions;

    public static class LastChangedListExtensions
    {
        public static IServiceCollection ConfigureLastChangedList(
            this IServiceCollection serviceCollection,
            string connectionString,
            string datadogServiceName,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<LastChangedListModule>();

            var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
            if (hasConnectionString)
            {
                RunOnSqlServer(datadogServiceName, serviceCollection, loggerFactory, connectionString);
            }
            else
            {
                RunInMemoryDb(serviceCollection, loggerFactory, logger);
            }

            logger.LogInformation(
                "Added {Context} to services:" +
                Environment.NewLine +
                "\tSchema: {Schema}" +
                Environment.NewLine +
                "\tTableName: {TableName}",
                nameof(LastChangedListContext), LastChangedListContext.Schema, LastChangedListContext.MigrationsHistoryTable);

            return serviceCollection;
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
            ILogger logger)
        {
            services
                .AddDbContext<LastChangedListContext>(options => options
                    .UseLoggerFactory(loggerFactory)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()));

            logger.LogWarning("Running InMemory for {Context}!", nameof(LastChangedListContext));
        }
    }
}
