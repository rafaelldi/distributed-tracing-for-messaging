using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using static masstransit_instrumentation.MassTransitInstrumentationActivitySource;

namespace masstransit_instrumentation.Filters
{
    public class PublishActivityFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            using var activity = Source.StartActivity("send", ActivityKind.Producer);

            activity?.SetTag("messaging.system", "rabbitmq");

            context.Headers.Set(Headers.TraceParent, activity?.Id);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("Publish activity");
        }
    }
}