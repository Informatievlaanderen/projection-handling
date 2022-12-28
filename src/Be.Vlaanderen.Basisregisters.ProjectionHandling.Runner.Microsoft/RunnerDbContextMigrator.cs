namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Microsoft
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Microsoft.Data.SqlClient;
    using global::Microsoft.Extensions.Logging;
    using MigrationExtensions;
    using Polly;

    public interface IRunnerDbContextMigrator
    {
        Task MigrateAsync(CancellationToken cancellationToken);
    }

    public class RunnerDbContextMigrator<TContext> : IRunnerDbContextMigrator
        where TContext : RunnerDbContext<TContext>
    {
        private const int RetryCount = 5;
        private readonly Func<TContext> _createContext;
        private readonly ILogger<RunnerDbContextMigrator<TContext>> _logger;

        public RunnerDbContextMigrator(Func<TContext> createContext, ILoggerFactory loggerFactory)
        {
            _createContext = createContext;
            _logger = loggerFactory?.CreateLogger<RunnerDbContextMigrator<TContext>>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            await Policy
                .Handle<SqlException>()
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
                        using (var migrationContext = _createContext())
                            await migrationContext.MigrateAsync(cancellationToken);
                    },
                    cancellationToken
                );
        }
    }
}
