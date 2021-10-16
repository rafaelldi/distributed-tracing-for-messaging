using contracts;
using MassTransit;
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
                                x.UsingRabbitMq();
                                x.AddRequestClient<GreetingRequest>();
                            })
                            .AddMassTransitHostedService();

                        services.AddOpenTelemetryTracing(x => x
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("api"))
                            .AddAspNetCoreInstrumentation()
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