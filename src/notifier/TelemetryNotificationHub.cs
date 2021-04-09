using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using notifier.Models;

namespace notifier
{
    public interface ITelemetryNotificationHub
    {
        Task BroadcastTelemetryUpdate(IEnumerable<DeviceTelemetry> metrics);
    }

    public class TelemetryNotificationHub : Hub<ITelemetryNotificationHub>
    {
        public async Task BroadcastTelemetryUpdate(IEnumerable<DeviceTelemetry> metrics)
        {
            await Clients.All.BroadcastTelemetryUpdate(metrics);
        }
    }
}
