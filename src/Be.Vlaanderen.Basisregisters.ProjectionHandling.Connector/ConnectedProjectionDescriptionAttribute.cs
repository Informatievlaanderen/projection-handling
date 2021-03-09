namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConnectedProjectionDescriptionAttribute : Attribute
    {
        public string Value { get;  }

        public ConnectedProjectionDescriptionAttribute(string projectDescription)
        {
            if (string.IsNullOrWhiteSpace(projectDescription))
                throw new ArgumentNullException(nameof(projectDescription));

            Value = projectDescription;
        }

        public static implicit operator string(ConnectedProjectionDescriptionAttribute attribute) => attribute.Value;
    }
}
