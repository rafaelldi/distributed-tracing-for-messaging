using System.Threading.Tasks;
using contracts;
using MassTransit;

namespace worker
{
    public class GreetingConsumer : IConsumer<GreetingRequest>
    {
        public async Task Consume(ConsumeContext<GreetingRequest> context)
        {
            await context.RespondAsync(new GreetingResponse($"Hello, {context.Message.Name}!"));
        }
    }
}