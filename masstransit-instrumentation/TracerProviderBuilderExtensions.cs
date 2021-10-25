using OpenTelemetry.Trace;

namespace masstransit_instrumentation
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddMassTransitSource(this TracerProviderBuilder builder) =>
            builder.AddSource(MassTransitInstrumentationActivitySource.ActivitySourceName);

        public static TracerProviderBuilder AddMassTransitLegacySource(this TracerProviderBuilder builder) =>
            builder
                .AddLegacySource("MassTransit.Consumer.Consume")
                .AddLegacySource("MassTransit.Transport.Send")
                .AddLegacySource("MassTransit.Transport.Receive")
                .AddLegacySource("MassTransit.Consumer.Handle");
    }
}