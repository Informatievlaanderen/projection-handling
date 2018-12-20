namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class TaskExtensions
    {
        public static async Task ExecuteAsync(this IEnumerable<Task> enumerable, CancellationToken cancellationToken)
        {
            foreach (var task in enumerable)
            {
                await task.ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                    return;
            }
        }
    }
}
