namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Model
{
    using System;

    public class LastChangedRecord
    {
        public string Id { get; set; }
        public string? CacheKey { get; set; }
        public string? Uri { get; set; }
        public string? AcceptType { get; set; }

        public long Position { get; set; }
        public long LastPopulatedPosition { get; set; }

        public int ErrorCount { get; set; }
        public DateTimeOffset? LastError { get; set; }
        public string? LastErrorMessage { get; set; }
    }
}
