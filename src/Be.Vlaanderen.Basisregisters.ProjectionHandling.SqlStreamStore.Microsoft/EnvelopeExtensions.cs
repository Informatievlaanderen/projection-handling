namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Microsoft
{
    using EventHandling;
    using global::Microsoft.Extensions.DependencyInjection;

    public static class EnvelopeExtensions
    {
        public static IServiceCollection ConfigureEnvelope(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<EnvelopeFactory>(_ => new EnvelopeFactory(_.GetRequiredService<EventMapping>(), _.GetRequiredService<EventDeserializer>()));
            
            return serviceCollection;
        }
    }
}
