using Framework.AzureSignalRLib;
using Microsoft.Extensions.Configuration;
using System;

namespace AzureSignalRClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddEnvironmentVariables()
          .Build();
            var connectionString = config.GetValue<string>("ConnectionString");
            var signalR = new AzureSignalR(connectionString);
            signalR.SendAsync(Hubs.OrderHub+ "_"+config.GetValue<string>("UserId"), "", Methods.brodCastBotOrder.ToString(), "Welcome").Wait();
            Console.ReadLine();

        }
    }
}
