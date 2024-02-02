namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Connector;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Polly;

    public abstract class LastChangedListConnectedProjection : ConnectedProjection<LastChangedListContext>
    {
        protected abstract string BuildCacheKey(AcceptType acceptType, string identifier);
        protected abstract string BuildUri(AcceptType acceptType, string identifier);

        private readonly AcceptType[] _supportedAcceptTypes;
        private readonly int _commandTimeoutInSeconds;
        private readonly ICacheValidator? _cacheValidator = null;
        private readonly int _cacheCheckIntervalInSeconds;

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes)
            : this(supportedAcceptTypes, 300) {}

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes, int commandTimeoutInSeconds)
        {
            _supportedAcceptTypes = supportedAcceptTypes;
            _commandTimeoutInSeconds = commandTimeoutInSeconds;
        }

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes, ICacheValidator cacheValidator)
            : this(supportedAcceptTypes, 300, cacheValidator, 5) {}

        protected LastChangedListConnectedProjection(AcceptType[] supportedAcceptTypes, int commandTimeoutInSeconds, ICacheValidator cacheValidator, int cacheCheckIntervalInSeconds)
        {
            _supportedAcceptTypes = supportedAcceptTypes;
            _commandTimeoutInSeconds = commandTimeoutInSeconds;
            _cacheValidator = cacheValidator;
            _cacheCheckIntervalInSeconds = cacheCheckIntervalInSeconds;
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
            await WaitTillCanCache(position, cancellationToken);

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

        private async Task WaitTillCanCache(long position, CancellationToken ct)
        {
            if (_cacheValidator is null)
            {
                return;
            }

            await Policy
                .HandleResult<bool>(isValid => !isValid)
                .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(_cacheCheckIntervalInSeconds))
                .ExecuteAsync(async () => await _cacheValidator.CanCache(position, ct));
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
