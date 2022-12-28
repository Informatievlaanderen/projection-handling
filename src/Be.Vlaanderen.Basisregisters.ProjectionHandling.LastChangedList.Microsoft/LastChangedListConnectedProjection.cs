namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Microsoft
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using global::Microsoft.EntityFrameworkCore;
    using Model;

    public abstract class LastChangedListConnectedProjection : ConnectedProjection<LastChangedListContext>
    {
        protected abstract string BuildCacheKey(AcceptType acceptType, string identifier);
        protected abstract string BuildUri(AcceptType acceptType, string identifier);

        private readonly AcceptType[] _supportedAcceptTypes;
        private readonly int _commandTimeoutInSeconds;

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes) : this(supportedAcceptTypes, 300) {}

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes, int commandTimeoutInSeconds)
        {
            _supportedAcceptTypes = supportedAcceptTypes;
            _commandTimeoutInSeconds = commandTimeoutInSeconds;
        }

        protected async Task<IEnumerable<LastChangedRecord>> GetLastChangedRecords(
            string identifier,
            LastChangedListContext context,
            CancellationToken cancellationToken)
        {
            context.Database.SetCommandTimeout(_commandTimeoutInSeconds);
            var attachedRecords = new List<LastChangedRecord>();

            // Create a record for every type that our API accepts.
            foreach (var acceptType in _supportedAcceptTypes)
            {
                var shortenedApplicationType = acceptType.ToString().ToLowerInvariant();
                var id = $"{identifier}.{shortenedApplicationType}";

                var record = await context
                    .LastChangedList
                    .FindAsync(id, cancellationToken: cancellationToken);

                if (record != null)
                    attachedRecords.Add(record);
            }

            return attachedRecords;
        }

        protected async Task<IEnumerable<LastChangedRecord>> GetLastChangedRecordsAndUpdatePosition(
            string identifier,
            long position,
            LastChangedListContext context,
            CancellationToken cancellationToken)
        {
            context.Database.SetCommandTimeout(_commandTimeoutInSeconds);
            var attachedRecords = new List<LastChangedRecord>();

            // Create a record for every type that our API accepts.
            foreach (var acceptType in _supportedAcceptTypes)
            {
                var shortenedApplicationType = acceptType.ToString().ToLowerInvariant();
                var id = $"{identifier}.{shortenedApplicationType}";

                var record = await context
                    .LastChangedList
                    .FindAsync(id, cancellationToken: cancellationToken);

                if (record == null)
                {
                    record = new LastChangedRecord
                    {
                        Id = id,
                        CacheKey = BuildCacheKey(acceptType, identifier),
                        Uri = BuildUri(acceptType, identifier),
                        AcceptType = GetApplicationType(acceptType)
                    };

                    await context.LastChangedList.AddAsync(record, cancellationToken);
                }

                record.Position = position;
                attachedRecords.Add(record);
            }

            return attachedRecords;
        }

        private static string GetApplicationType(AcceptType acceptType)
        {
            return acceptType switch
            {
                AcceptType.Json => "application/json",
                AcceptType.JsonLd => "application/ld+json",
                AcceptType.Xml => "application/xml",
                _ => string.Empty
            };
        }
    }
}
