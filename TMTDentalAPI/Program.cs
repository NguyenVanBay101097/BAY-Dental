using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TMTDentalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //    var seed = args.Any(x => x == "/seed");
        //    if (seed) args = args.Except(new[] { "/seed" }).ToArray();
        //    var host = CreateWebHostBuilder(args).Build();
        //    if (seed)
        //    {
        //        var config = host.Services.GetRequiredService<IConfiguration>();
        //        var connectionString = config.GetConnectionString("CatalogConnection");
        //        SeedData.EnsureSeedData(connectionString);
        //        return;
        //    }

        //    host.Run();
        //}

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .ConfigureAppConfiguration((hostingContext, config) =>
        //    {
        //        var env = hostingContext.HostingEnvironment;
        //        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        //    })
        //    .UseStartup<Startup>();
    }
}
