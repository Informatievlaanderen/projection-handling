namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Microsoft
{
    using DependencyInjection;
    using EventHandling;
    using global::Microsoft.Extensions.DependencyInjection;

    public class EnvelopeModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddTransient<EnvelopeFactory>(_ => new EnvelopeFactory(_.GetRequiredService<EventMapping>(), _.GetRequiredService<EventDeserializer>()));
        }
    }
}
