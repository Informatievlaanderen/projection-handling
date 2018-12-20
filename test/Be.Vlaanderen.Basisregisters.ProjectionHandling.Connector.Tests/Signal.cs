namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Tests
{
    public class Signal
    {
        public Signal() => IsSet = false;

        public void Set() => IsSet = true;

        public bool IsSet { get; private set; }
    }
}
