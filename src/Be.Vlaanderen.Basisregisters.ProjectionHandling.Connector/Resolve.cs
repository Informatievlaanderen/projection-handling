namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the available <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolvers</see>.
    /// </summary>
    public static class Resolve
    {
        /// <summary>
        /// Resolves the <see cref="ConnectedProjectionHandler{TConnection}">handlers</see> that match the type of the message exactly.
        /// </summary>
        /// <param name="handlers">The set of resolvable handlers.</param>
        /// <returns>A <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolver</see>.</returns>
        public static ConnectedProjectionHandlerResolver<TConnection> WhenEqualToHandlerMessageType<TConnection>(ConnectedProjectionHandler<TConnection>[] handlers)
        {
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));

            var cache = handlers.
                GroupBy(handler => handler.Message).
                ToDictionary(group => group.Key, group => group.ToArray());

            return message =>
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                return cache.TryGetValue(message.GetType(), out var result) ?
                    result :
                    new ConnectedProjectionHandler<TConnection>[0];
            };
        }

        /// <summary>
        /// Resolves the <see cref="ConnectedProjectionHandler{TConnection}">handlers</see> to which the message instance is assignable.
        /// </summary>
        /// <param name="handlers">The set of resolvable handlers.</param>
        /// <returns>A <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolver</see>.</returns>
        public static ConnectedProjectionHandlerResolver<TConnection> WhenAssignableToHandlerMessageType<TConnection>(ConnectedProjectionHandler<TConnection>[] handlers)
        {
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));

            var cache = new Dictionary<Type, ConnectedProjectionHandler<TConnection>[]>();

            return message =>
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (!cache.TryGetValue(message.GetType(), out var result))
                {
                    result = Array.FindAll(handlers, handler => handler.Message.IsInstanceOfType(message));
                    cache.Add(message.GetType(), result);
                }

                return result;
            };
        }
    }
}
