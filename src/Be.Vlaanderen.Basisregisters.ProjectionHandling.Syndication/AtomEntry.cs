namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using Microsoft.SyndicationFeed;

    public class AtomEntry
    {
        public IAtomEntry FeedEntry { get; }
        public object Content { get; }

        public AtomEntry(IAtomEntry entry, object content)
        {
            FeedEntry = entry ?? throw new ArgumentNullException(nameof(entry));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    public class AtomEntry<TContent>
    {
        public IAtomEntry FeedEntry { get; }
        public TContent Content { get; }

        public AtomEntry(AtomEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            FeedEntry = entry.FeedEntry;
            Content = (TContent)entry.Content;
        }
    }
}
