using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TelemetryClient.Settings;
using Random = System.Random;

namespace TelemetryClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            var connection = new HubConnectionBuilder()
                           .WithUrl(configuration.TelemetryHubUrl)
                           .WithAutomaticReconnect()
                           .ConfigureLogging(logging =>
                           {
                               logging.AddConsole();
                               logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                           })
                           .Build();

            connection.On<object>("BroadcastTelemetryUpdate", m =>
            {
                Console.WriteLine(m);
            });

            await connection.StartAsync();

            string deviceId = "OddTemperatureGenerator";

            if (new Random().Next(2) % 2 == 0)
            {
                deviceId = "EvenTemperatureGenerator";
            }

            await connection.SendAsync("SubscribeToDevice", deviceId);

            Console.WriteLine("Connected to the Telemetry notification service! - Press any key to quit");
            Console.WriteLine("Telemetry Updates that are send are displayed here:");

            Console.ReadLine();
        }

        private static SignalRSettings GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                                .AddJsonFile("appsettings.json", false)
                                .AddJsonFile("appsettings.development.json", true)
                                .Build();

            var settings = new SignalRSettings();

            configuration.Bind("SignalR", settings);

            return settings;
        }
    }
}
