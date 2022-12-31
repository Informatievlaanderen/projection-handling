namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Microsoft
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class AtomResolveHandlerException : Exception
    {
        public AtomResolveHandlerException()
        { }

        private AtomResolveHandlerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public AtomResolveHandlerException(string? message)
            : base(message)
        { }

        public AtomResolveHandlerException(string? message, Exception? innerException)
            : base(message, innerException)
        { }
    }
}
