using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.TenantData;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TMTDentalAdmin
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.File(@"logs\log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var host = CreateWebHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var context = services.GetService<TenantDbContext>();
                        context.Database.Migrate();

                        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationAdminUser>>();
                        var admin = userMgr.FindByNameAsync("admin").Result;
                        if (admin == null)
                        {
                            admin = new ApplicationAdminUser
                            {
                                UserName = "admin",
                                Email = "tai161292@gmail.com"
                            };
                            var result = userMgr.CreateAsync(admin, "Pass123$").Result;
                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }

                            Console.WriteLine("admin created");
                        }
                        else
                        {
                            Console.WriteLine("admin already exists");
                        }

                        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                        var tenants = context.Tenants.ToListAsync().Result;
                        foreach (var tenant in tenants)
                        {
                            CatalogDbContext catalogContext = new CatalogDbContext(new DbContextOptions<CatalogDbContext>(), tenant, configuration);
                            if (catalogContext.Database.CanConnect())
                            {
                                if (catalogContext.Database.GetPendingMigrations().Any())
                                    catalogContext.Database.Migrate();

                                if (tenant.ActiveCompaniesNbr == 0)
                                    tenant.ActiveCompaniesNbr = catalogContext.Companies.Where(x => x.Active).Count();
                            }
                        }
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
              .UseSerilog()
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var env = hostingContext.HostingEnvironment;
                 config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
             })
                .UseStartup<Startup>();
    }
}
