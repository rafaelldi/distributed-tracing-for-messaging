using System.Diagnostics;

namespace masstransit_instrumentation
{
    public static class MassTransitInstrumentationActivitySource
    {
        public const string ActivitySourceName = "MassTransitInstrumentation";
        public static readonly ActivitySource Source = new(ActivitySourceName);
    }
}