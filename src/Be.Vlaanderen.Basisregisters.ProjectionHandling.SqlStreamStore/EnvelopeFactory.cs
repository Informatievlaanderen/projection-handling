namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore
{
    using System;
    using System.Collections.Generic;
    using EventHandling;
    using global::SqlStreamStore.Streams;

    public class EnvelopeFactory
    {
        private readonly EventMapping _eventMapping;
        private readonly EventDeserializer _eventDeserializer;

        public EnvelopeFactory(EventMapping eventMapping, EventDeserializer eventDeserializer)
        {
            _eventMapping = eventMapping;
            _eventDeserializer = eventDeserializer;
        }

        public object Create(StreamMessage message)
        {
            var @event = Deserialize(message);

            var deserializedMetadata = (Dictionary<string, object>?)_eventDeserializer.DeserializeObject(message.JsonMetadata, typeof(Dictionary<string, object>));
            var metadata = deserializedMetadata != null
                ? new Dictionary<string, object>(deserializedMetadata, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            metadata[Envelope.StreamIdMetadataKey] = message.StreamId;
            metadata[Envelope.PositionMetadataKey] = message.Position;
            metadata[Envelope.EventNameMetadataKey] = message.Type;
            metadata[Envelope.CreatedUtcMetadataKey] = message.CreatedUtc;

            var envelope =
                new Envelope(
                        @event,
                        metadata)
                    .ToGenericEnvelope();

            return envelope;
        }

        public bool TryCreate(StreamMessage message, out object? envelope)
        {
            envelope = _eventMapping.HasEventType(message.Type) ? Create(message) : null;

            return envelope != null;
        }

        private object? Deserialize(StreamMessage message)
        {
            var eventData = message.GetJsonData().GetAwaiter().GetResult();
            var eventType = _eventMapping.GetEventType(message.Type);
            return _eventDeserializer.DeserializeObject(eventData, eventType);
        }
    }
}
