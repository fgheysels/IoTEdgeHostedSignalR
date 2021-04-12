using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace datasimulator
{
    class Program
    {
        private static readonly Random Randomizer = new Random();

        static async Task Main(string[] args)
        {
            var moduleClient = await Init();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

            while (cts.IsCancellationRequested == false)
            {
                var msg = GenerateMessage();

                await moduleClient.SendEventAsync("data_output", msg, cts.Token);

                await Task.Delay(500, cts.Token);
            }

            await WhenCancelled(cts.Token);
        }

        private static Message GenerateMessage()
        {
            var temp = Randomizer.Next(-5, 15);

            var deviceId = (temp % 2 == 0) ? "EvenTemperatureGenerator" : "OddTemperatureGenerator";

            var json = $"{{ \"deviceId\": \"{deviceId}\", \"temperature\": {temp} }}";

            return new Message(System.Text.Encoding.ASCII.GetBytes(json));
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task<ModuleClient> Init()
        {
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            return ioTHubModuleClient;
        }
    }
}
