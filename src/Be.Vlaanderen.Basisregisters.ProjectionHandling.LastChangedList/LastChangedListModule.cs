namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System;
    using Autofac;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Runner.SqlServer.MigrationExtensions;

    public class LastChangedListModule : Module
    {
        public LastChangedListModule(
            string connectionString,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<LastChangedListModule>();

            var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
            if (hasConnectionString)
                RunOnSqlServer(services, loggerFactory, connectionString);
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
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            string backofficeProjectionsConnectionString)
        {
            services
                .AddDbContext<LastChangedListContext>((_, options) => options
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(
                        backofficeProjectionsConnectionString,
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
                    .UseInMemoryDatabase(Guid.NewGuid().ToString(), sqlServerOptions => { }));

            logger.LogWarning("Running InMemory for {Context}!", nameof(LastChangedListContext));
        }
    }
}
