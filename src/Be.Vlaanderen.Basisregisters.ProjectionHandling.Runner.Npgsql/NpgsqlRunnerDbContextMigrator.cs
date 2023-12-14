namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Npgsql
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Npgsql;
    using Microsoft.Extensions.Logging;
    using MigrationExtensions;
    using Polly;

    public class NpgsqlRunnerDbContextMigrator<TContext> : IRunnerDbContextMigrator
        where TContext : RunnerDbContext<TContext>
    {
        private const int RetryCount = 5;
        private readonly Func<TContext> _createContext;
        private readonly ILogger<NpgsqlRunnerDbContextMigrator<TContext>> _logger;

        public NpgsqlRunnerDbContextMigrator(Func<TContext> createContext, ILoggerFactory loggerFactory)
        {
            _createContext = createContext;
            _logger = loggerFactory?.CreateLogger<NpgsqlRunnerDbContextMigrator<TContext>>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            await Policy
                .Handle<NpgsqlException>()
                .WaitAndRetryAsync(
                    RetryCount,
                    retryAttempt =>
                    {
                        var value = Math.Pow(2, retryAttempt) / 4;
                        var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                        _logger.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                        return TimeSpan.FromSeconds(randomValue);
                    }
                )
                .ExecuteAsync(
                    async token =>
                    {
                        _logger.LogInformation("Running EF Migrations for {ContextType}", typeof(TContext).Name);
                        await using var migrationContext = _createContext();
                        await migrationContext.MigrateAsync(cancellationToken);
                    },
                    cancellationToken
                );
        }
    }
}
