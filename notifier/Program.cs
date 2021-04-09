using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;

namespace notifier
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder();
            await host.Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                       .ConfigureIoTEdgeModuleClient(TransportType.Mqtt_Tcp_Only)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.ConfigureKestrel(kestrelServerOptions => kestrelServerOptions.AddServerHeader = false)
                                     .UseUrls("http://*:8080")
                                     .ConfigureLogging(logging => logging.AddConsole())
                                     .UseStartup<Startup>();
                       });
        }
    }
}
