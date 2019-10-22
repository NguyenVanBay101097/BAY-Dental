using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TenantDbContext>(c => c.UseSqlServer(Configuration.GetConnectionString("TenantConnection")));

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
                            var tenant = services.BuildServiceProvider().GetService<AppTenant>();
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
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddSingleton<IMailSender, SendGridSender>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppTenantProfile());
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
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
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
