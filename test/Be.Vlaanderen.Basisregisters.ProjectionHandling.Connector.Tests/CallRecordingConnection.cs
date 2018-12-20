namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class CallRecordingConnection
    {
        private readonly List<Tuple<int, object, CancellationToken>> _calls;

        public CallRecordingConnection()
        {
            _calls = new List<Tuple<int, object, CancellationToken>>();
        }

        public void RecordCall(int handler, object message, CancellationToken token)
        {
            _calls.Add(new Tuple<int, object, CancellationToken>(handler, message, CancellationToken.None));
        }

        public Tuple<int, object, CancellationToken>[] RecordedCalls => _calls.ToArray();
    }
}
