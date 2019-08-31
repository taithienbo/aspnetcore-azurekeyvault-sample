using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace AccessKeyVaultUsingCertificateAspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
 
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((host, logging) =>
            {
                logging.ClearProviders()
                .AddConfiguration(host.Configuration.GetSection("Logging"));
            })
            .UseNLog()
            .UseStartup<Startup>();
    }
}
