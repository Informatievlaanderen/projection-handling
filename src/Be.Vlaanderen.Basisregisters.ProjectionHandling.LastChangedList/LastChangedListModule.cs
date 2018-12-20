namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System;
    using System.Data.SqlClient;
    using Autofac;
    using DataDog.Tracing.Sql.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class LastChangedListModule : Module
    {
        public LastChangedListModule(
            string connectionString,
            string datadogServiceName,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<LastChangedListModule>();

            var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
            if (hasConnectionString)
                RunOnSqlServer(datadogServiceName, services, loggerFactory, connectionString);
            else
                RunInMemoryDb(services, loggerFactory, logger);

            logger.LogInformation(
                "Added {Context} to services:" +
                Environment.NewLine +
                "\tSchema: {Schema}" +
                Environment.NewLine +
                "\tTableName: {TableName}",
                nameof(LastChangedListContext), LastChangedListContext.Schema, LastChangedListContext.MigrationsHistoryTable);
        }

        private static void RunOnSqlServer(
            string datadogServiceName,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            string backofficeProjectionsConnectionString)
        {
            services
                .AddScoped(s => new TraceDbConnection(
                    new SqlConnection(backofficeProjectionsConnectionString),
                        datadogServiceName))
                .AddDbContext<LastChangedListContext>((provider, options) => options
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(provider.GetRequiredService<TraceDbConnection>(), sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.MigrationsHistoryTable(LastChangedListContext.MigrationsHistoryTable, LastChangedListContext.Schema);
                    }));
        }

        private static void RunInMemoryDb(
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            ILogger<LastChangedListModule> logger)
        {
            services
                .AddDbContext<LastChangedListContext>(options => options
                    .UseLoggerFactory(loggerFactory)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString(), sqlServerOptions => { }));

            logger.LogWarning("Running InMemory for {Context}!", nameof(LastChangedListContext));
        }
    }
}
