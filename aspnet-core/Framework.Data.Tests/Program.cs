using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Framework.Data.Tests
{
    class Program
    {
        public static IConfigurationRoot configuration;
        static async Task Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            //.Enrich.FromLogContext()
            //.CreateLogger();
            //Console.WriteLine("Hello World!");

            //var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile($"appsettings.json", true, true)
            //    .AddJsonFile($"appsettings.{env}.json", true, true);

            //var config = builder.Build();

            IConfiguration Configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json"/*, optional: true, reloadOnChange: true*/)
                        //.AddEnvironmentVariables()
                        //.AddCommandLine(args)
                        .Build();
            
            var csomosdb = Configuration.GetSection("CosmosDBSettings");
            Console.WriteLine(Configuration["CosmosDBSettings:EndPoint"]);
            Console.WriteLine(csomosdb["AuthKey"]);
            Console.WriteLine(csomosdb["Database"]);
            Console.WriteLine(csomosdb["ItemsCollection"]);

            //var obj = new DocumentDBHelper<ConversationDb>(CollectionTypes.ItemsCollection);
            //await obj.CreateItemTestAsync(new ConversationDb()
            //{
            //    ID = Guid.NewGuid().ToString(),
            //    Name = "Mustafa Snaid"
            //});
        }
    }
}
