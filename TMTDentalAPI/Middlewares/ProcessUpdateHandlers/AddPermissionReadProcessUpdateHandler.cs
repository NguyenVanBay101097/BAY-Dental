using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class AddPermissionReadProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.1";
        private IServiceScopeFactory _serviceScopeFactory;
        public AddPermissionReadProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    //Version version1 = new Version(_version);
                    //Version version2 = new Version(tenant.Version);
                    //if (version2.CompareTo(version1) >= 0)
                    //    return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var cacheObj = scope.ServiceProvider.GetService<IMyCache>();

                var permissions = new string[]
                {
                    "Card.Type.Read",
                    "Card.Card.Read",
                    "ServiceCard.Type.Read",
                    "ServiceCard.Card.Read",
                };

                var roles = context.Roles
                    .Include(x => x.Functions)
                    .ToList();

                foreach(var role in roles)
                {
                    foreach (var permission in permissions)
                    {
                        if (!role.Functions.Any(x => x.Func == permission))
                            role.Functions.Add(new ApplicationRoleFunction { Func = permission });
                    }
                }

                context.SaveChanges();

                cacheObj.RemoveByPattern($"{(tenant != null ? tenant.Hostname : "localhost")}-permissions");
            }

            return Task.CompletedTask;
        }
    }
}
