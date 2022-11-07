namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac
{
    using EventHandling;
    using global::Autofac;

    public class EnvelopeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => new EnvelopeFactory(c.Resolve<EventMapping>(), c.Resolve<EventDeserializer>()))
                .As<EnvelopeFactory>();
        }
    }
}
