using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using notifier.Models;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace notifier
{
    public class TelemetryProcessor : BackgroundService
    {
        private readonly ModuleClient _moduleClient;
        private readonly IHubContext<TelemetryNotificationHub, ITelemetryNotificationHub> _telemetryNotifier;
        private readonly ILogger<TelemetryProcessor> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryProcessor"/> class.
        /// </summary>
        public TelemetryProcessor(ModuleClient moduleClient, IHubContext<TelemetryNotificationHub, ITelemetryNotificationHub> telemetryNotifier, ILogger<TelemetryProcessor> logger)
        {
            _moduleClient = moduleClient;
            _telemetryNotifier = telemetryNotifier;
            _logger = logger;
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _moduleClient.SetInputMessageHandlerAsync("telemetry_input", async (m, s) =>
            {
                var json = Encoding.ASCII.GetString(m.GetBytes());

                var data = JsonConvert.DeserializeObject<DeviceTelemetry>(json);

                // It's a bit silly to put the logic to which groups we need to send the messages here,
                // since this has also been abstracted away by the Hub implementation.
                await _telemetryNotifier.Clients.Groups(data.DeviceId).BroadcastTelemetryUpdate(new[] { data });

                _logger.LogInformation($"Broadcasted {json}");

                return MessageResponse.Completed;
            }, null);

            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO: in a future version, collect  telemetry and only notify via SignalR every x seconds.
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
