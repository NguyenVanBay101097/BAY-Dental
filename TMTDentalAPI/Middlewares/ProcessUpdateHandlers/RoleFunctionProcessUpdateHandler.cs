using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class RoleFunctionProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public RoleFunctionProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    Version version1 = new Version(_version);
                    Version version2 = new Version(tenant.Version);
                    if (version2.CompareTo(version1) >= 0)
                        return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var _webHostEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\features.json");
                using (var reader = new StreamReader(filePath))
                {
                    var fileContent = reader.ReadToEnd();
                    var features = JsonConvert.DeserializeObject<List<PermissionTreeV2GroupViewModel>>(fileContent);
                    var allOps = features.SelectMany(x => x.Functions).SelectMany(x => x.Ops).Select(x => x.Permission).ToList();
                    var roles = context.Roles.Include(x => x.Functions).ToList();
                    foreach(var role in roles)
                    {
                        foreach(var function in role.Functions.ToList())
                        {
                            var ops = allOps.Where(x => x.IndexOf(function.Func) == 0).ToList();
                            foreach(var op in ops)
                            {
                                if (!role.Functions.Any(x => x.Func == op))
                                    role.Functions.Add(new ApplicationRoleFunction { Func = op });
                            }
                        }
                    }

                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
