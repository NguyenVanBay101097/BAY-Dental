using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Middlewares;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Mapping;

namespace TMTDentalAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TenantDbContext>(c => c.UseSqlServer(Configuration.GetConnectionString("TenantConnection")));

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            // configure jwt authentication
            var appSettingsSection = Configuration.GetSection("AdminAppSettings");
            services.Configure<AdminAppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AdminAppSettings>();

            services.AddIdentity<ApplicationAdminUser, IdentityRole>(config =>
            {
                config.Password.RequireLowercase = false;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Lockout = new LockoutOptions
                {
                    DefaultLockoutTimeSpan = new TimeSpan(),
                    MaxFailedAccessAttempts = 5,
                };
            })
               .AddEntityFrameworkStores<TenantDbContext>()
               .AddDefaultTokenProviders();

            // configure jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                        {
                            List<SecurityKey> keys = new List<SecurityKey>();
                            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                            var signingKey = new SymmetricSecurityKey(key);
                            keys.Add(signingKey);
                            return keys;
                        }
                    };
                });


            services.AddScoped<IDbContext, TenantDbContext>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAdminBaseService<>), typeof(AdminBaseService<>));
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IEmployeeAdminService, EmployeeAdminService>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddSingleton<IMailSender, SendGridSender>();
            services.AddScoped<ITenantExtendHistoryService, TenantExtendHistoryService>();
            services.AddSingleton<UpdateExpiredDateTenantService>();
            //services.AddCronJob<ScheduleJobService>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"55 14 * * *"; //chay moi ngay voi so gio dc set san
            //    c.ConnectionStrings = Configuration.GetConnectionString("TenantConnection");
            //    c.appSettings = appSettings;
            //});

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppTenantProfile());
                mc.AddProfile(new TenantExtendHistoryProfile());
                mc.AddProfile(new EmployeeAdminProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
