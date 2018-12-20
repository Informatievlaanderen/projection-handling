namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Polly;

    public class MigrationsLogger { }

    public static class LastChangedListMigrationsHelper
    {
        public static async Task RunAsync(
            string connectionString,
            ILoggerFactory loggerFactory = null,
            CancellationToken cancellationToken = default)
        {
            var logger = loggerFactory?.CreateLogger<MigrationsLogger>();

            await Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(
                    5,
                    retryAttempt =>
                    {
                        var value = Math.Pow(2, retryAttempt) / 4;
                        var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                        logger?.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                        return TimeSpan.FromSeconds(randomValue);
                    })
                .ExecuteAsync(async ct =>
                    {
                        logger?.LogInformation("Running EF Migrations.");
                        await RunInternal(connectionString, loggerFactory, ct);
                    },
                    cancellationToken);
        }

        private static async Task RunInternal(string connectionString, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
        {
            var migratorOptions = new DbContextOptionsBuilder<LastChangedListContext>()
                .UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.MigrationsHistoryTable(LastChangedListContext.MigrationsHistoryTable, LastChangedListContext.Schema);
                    });

            if (loggerFactory != null)
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);

            using (var migrator = new LastChangedListContext(migratorOptions.Options))
                await migrator.Database.MigrateAsync(cancellationToken);
        }
    }
}
