namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System;

    [Serializable]
    public class ProjectionItemNotFoundException<TProjection> : Exception
    {
        public ProjectionItemNotFoundException(string id)
            : base($"Item with id '{id}' not found in projection '{typeof(TProjection).FullName}'.") { }

        public ProjectionItemNotFoundException(string id, Exception inner)
            : base($"Item with id '{id}' not found in projection '{typeof(TProjection).FullName}'.", inner) { }
    }
}
