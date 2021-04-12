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
        Task SubscribeToDevice(string deviceName);

        Task BroadcastTelemetryUpdate(IEnumerable<DeviceTelemetry> metrics);
    }

    public class TelemetryNotificationHub : Hub<ITelemetryNotificationHub>
    {
        public async Task SubscribeToDevice(string deviceName)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, deviceName);
            
            Console.WriteLine($"Subscribed {Context.ConnectionId} to {deviceName}");
        }

        public async Task BroadcastTelemetryUpdate(IEnumerable<DeviceTelemetry> metrics)
        {
            var oddDeviceMetrics = metrics.Where(d => d.DeviceId == "OddDevice");
            var evenDeviceMetrics = metrics.Where(d => d.DeviceId == "EvenDevice");

            if (oddDeviceMetrics.Any())
            {
                await Clients.Groups("OddDevice").BroadcastTelemetryUpdate(oddDeviceMetrics);
            }

            if (evenDeviceMetrics.Any())
            {
                await Clients.Groups("EvenDevice").BroadcastTelemetryUpdate(evenDeviceMetrics);
            }
        }
    }
}
