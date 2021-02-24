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

namespace TMTDentalAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var seed = args.Any(x => x == "/seed");
            //if (seed) args = args.Except(new[] { "/seed" }).ToArray();
            //var host = CreateWebHostBuilder(args).Build();
            //if (seed)
            //{
            //    var config = host.Services.GetRequiredService<IConfiguration>();
            //    var connectionString = config.GetConnectionString("TenantConnection");
            //    SeedData.EnsureSeedData(connectionString);
            //    return;
            //}

            //host.Run();

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
                    foreach(var tenant in tenants)
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


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var env = hostingContext.HostingEnvironment;
                 config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
             })
                .UseStartup<Startup>();
    }
}
