namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConnectedProjectionNameAttribute : Attribute
    {
        public string Value { get;  }

        public ConnectedProjectionNameAttribute(string projectionName)
        {
            if (string.IsNullOrWhiteSpace(projectionName))
                throw new ArgumentNullException(nameof(projectionName));

            Value = projectionName;
        }

        public static implicit operator string(ConnectedProjectionNameAttribute attribute) => attribute.Value;
    }
}
