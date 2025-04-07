namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class AtomResolveHandlerException : Exception
    {
        public AtomResolveHandlerException()
        { }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
