namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Runtime.Serialization;

    public class AtomResolveHandlerException : Exception
    {
        public AtomResolveHandlerException() { }

        protected AtomResolveHandlerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public AtomResolveHandlerException(string? message) : base(message) { }

        public AtomResolveHandlerException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
