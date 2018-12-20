namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Envelope<TMessage> // Used by handlers
    {
        private readonly Envelope _envelope;

        public TMessage Message => (TMessage)_envelope.Message;

        public long Position => (long)_envelope.Metadata[Envelope.PositionMetadataKey];

        public string EventName => (string)_envelope.Metadata[Envelope.EventNameMetadataKey];

        public DateTime CreatedUtc => (DateTime)_envelope.Metadata[Envelope.CreatedUtcMetadataKey];

        public IReadOnlyDictionary<string, object> Metadata => _envelope.Metadata;

        // NOTE: If you ever rename the parameter name 'envelope', be sure to replace it in ToGenericEnvelope as well (reflection shit)!
        public Envelope(Envelope envelope) =>
            _envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
    }

    public class Envelope // Used by dispatchers
    {
        public const string PositionMetadataKey = "Position";
        public const string EventNameMetadataKey = "EventName";
        public const string CreatedUtcMetadataKey = "CreatedUtc";

        // Note we could precompute these factories for all known message types.
        private static readonly ConcurrentDictionary<Type, Func<Envelope, object>> Factories =
            new ConcurrentDictionary<Type, Func<Envelope, object>>();

        public object Message { get; }
        public IReadOnlyDictionary<string, object> Metadata { get; }

        public Envelope(object message, IReadOnlyDictionary<string, object> metadata)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public object ToGenericEnvelope()
        {
            var factory = Factories
                .GetOrAdd(Message.GetType(), typeOfMessage =>
                {
                    var parameter = Expression
                        .Parameter(typeof(Envelope), "envelope");

                    return Expression
                        .Lambda<Func<Envelope, object>>(
                            Expression.New(
                                typeof(Envelope<>)
                                    .MakeGenericType(typeOfMessage)
                                    .GetConstructors()
                                    .Single(),
                                parameter),
                            parameter)
                        .Compile();
                });

            return factory(this);
        }
    }
}
