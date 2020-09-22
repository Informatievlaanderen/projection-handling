namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Connector;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public abstract class LastChangedListConnectedProjection : ConnectedProjection<LastChangedListContext>
    {
        protected abstract string CacheKeyFormat { get; }
        protected abstract string UriFormat { get; }

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
                        CacheKey = string.Format(CacheKeyFormat, identifier, shortenedApplicationType),
                        Uri = string.Format(UriFormat, identifier),
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
            switch (acceptType)
            {
                case AcceptType.Json:
                    return "application/json";

                case AcceptType.JsonLd:
                    return "application/ld+json";

                case AcceptType.Xml:
                    return "application/xml";

                default:
                    return string.Empty;
            }
        }
    }
}
