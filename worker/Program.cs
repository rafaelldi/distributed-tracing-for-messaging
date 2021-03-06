using MassTransit;
using masstransit_instrumentation;
using masstransit_instrumentation.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddMassTransit(x =>
                        {
                            x.SetKebabCaseEndpointNameFormatter();
                            x.UsingRabbitMq((context, cfg) =>
                            {
                                //Uncomment for activities via filters
                                //cfg.UseSendFilter(typeof(SendActivityFilter<>), context);
                                //cfg.UseConsumeFilter(typeof(ConsumeActivityFilter<>), context);

                                cfg.ConfigureEndpoints(context);
                            });
                            x.AddConsumer<GreetingConsumer>();
                        })
                        .AddMassTransitHostedService()
                        .AddHostedService<MassTransitDiagnosticsHostedService>();

                    services.AddOpenTelemetryTracing(x => x
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("worker"))
                        //Uncomment for activities via filters
                        //.AddMassTransitSource()
                        .AddMassTransitLegacySource()
                        .AddJaegerExporter());
                });
    }
}