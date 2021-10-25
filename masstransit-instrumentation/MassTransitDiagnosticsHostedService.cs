using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace masstransit_instrumentation
{
    public class MassTransitDiagnosticsHostedService : IHostedService
    {
        private IDisposable _subscription;
        private IDisposable _massTransitSubscription;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription ??= DiagnosticListener.AllListeners.Subscribe(delegate(DiagnosticListener listener)
            {
                if (listener.Name == "MassTransit")
                {
                    _massTransitSubscription ??= listener.Subscribe();
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _massTransitSubscription?.Dispose();
            _subscription?.Dispose();
            return Task.CompletedTask;
        }
    }
}