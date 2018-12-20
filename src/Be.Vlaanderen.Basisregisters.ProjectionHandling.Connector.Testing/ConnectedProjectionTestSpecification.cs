namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent a <see cref="ConnectedProjection{TConnection}"/> test specification.
    /// </summary>
    /// <typeparam name="TConnection">The type of the connection.</typeparam>
    public class ConnectedProjectionTestSpecification<TConnection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedProjectionTestSpecification{TConnection}"/> class.
        /// </summary>
        /// <param name="resolver">The projection handler resolver.</param>
        /// <param name="messages">The messages to project.</param>
        /// <param name="verification">The verification method.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when
        /// <paramref name="resolver"/>
        /// or
        /// <paramref name="messages"/>
        /// or
        /// <paramref name="verification"/> is null.
        /// </exception>
        public ConnectedProjectionTestSpecification(ConnectedProjectionHandlerResolver<TConnection> resolver, object[] messages, Func<TConnection, CancellationToken, Task<VerificationResult>> verification)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            Verification = verification ?? throw new ArgumentNullException(nameof(verification));
        }

        /// <summary>
        /// Gets the projection handler resolver.
        /// </summary>
        /// <value>
        /// The projection handler resolver.
        /// </value>
        public ConnectedProjectionHandlerResolver<TConnection> Resolver { get; }

        /// <summary>
        /// Gets the messages to project.
        /// </summary>
        /// <value>
        /// The messages to project.
        /// </value>
        public object[] Messages { get; }

        /// <summary>
        /// Gets the verification method.
        /// </summary>
        /// <value>
        /// The verification method.
        /// </value>
        public Func<TConnection, CancellationToken, Task<VerificationResult>> Verification { get; }
    }
}
