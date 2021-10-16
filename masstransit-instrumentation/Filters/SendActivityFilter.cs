using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using static masstransit_instrumentation.MassTransitInstrumentationActivitySource;

namespace masstransit_instrumentation.Filters
{
    public class SendActivityFilter<T> : IFilter<SendContext<T>> where T : class
    {
        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            using var activity = Source.StartActivity("send", ActivityKind.Producer);

            activity?.SetTag("messaging.system", "rabbitmq");

            context.Headers.Set(Headers.TraceParent, activity?.Id);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("Send activity");
        }
    }
}