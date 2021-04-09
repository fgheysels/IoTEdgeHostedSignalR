using System;
using System.IO;

namespace TelemetryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
