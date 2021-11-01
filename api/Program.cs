using contracts;
using MassTransit;
using masstransit_instrumentation;
using masstransit_instrumentation.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddMassTransit(x =>
                            {
                                x.SetKebabCaseEndpointNameFormatter();
                                x.UsingRabbitMq((context, cfg) =>
                                {
                                    //Uncomment for activities via filters
                                    //cfg.UsePublishFilter(typeof(PublishActivityFilter<>), context);
                                });
                                x.AddRequestClient<GreetingRequest>();
                            })
                            .AddMassTransitHostedService()
                            .AddHostedService<MassTransitDiagnosticsHostedService>();

                        services.AddOpenTelemetryTracing(x => x
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("api"))
                            .AddAspNetCoreInstrumentation()
                            //Uncomment for activities via filters
                            //.AddMassTransitSource()
                            .AddMassTransitLegacySource()
                            .AddJaegerExporter());
                    });
                    webBuilder.Configure(builder =>
                    {
                        builder.UseRouting();
                        builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    });
                });
    }
}