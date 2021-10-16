using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using static masstransit_instrumentation.MassTransitInstrumentationActivitySource;

namespace masstransit_instrumentation.Filters
{
    public class ConsumeActivityFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            using var activity = context.Headers.TryGetHeader(Headers.TraceParent, out var parentId)
                ? Source.StartActivity("receive", ActivityKind.Consumer, (string)parentId)
                : Source.StartActivity("receive", ActivityKind.Consumer);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.operation", "receive");

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("Consume activity");
        }
    }
}