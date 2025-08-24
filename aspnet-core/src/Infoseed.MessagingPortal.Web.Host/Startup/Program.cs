using System.IO;
using Microsoft.AspNetCore.Hosting;
using Infoseed.MessagingPortal.Web.Helpers;

namespace Infoseed.MessagingPortal.Web.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
  .UseKestrel(opt =>
{
    opt.AddServerHeader = false;
    opt.Limits.MaxRequestLineSize = 16 * 1024;
    opt.Limits.MaxRequestBodySize = 104857600; // 100MB
})

                .UseSentry(o =>
                {
                    o.Dsn = "";
                    // When configuring for the first time, to see what the SDK is doing:
                    o.Debug = true;
                    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                    // We recommend adjusting this value in production.
                    o.TracesSampleRate = 1.0;
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIIS()
                .UseIISIntegration()
                .UseStartup<Startup>();
        }
    }
}
