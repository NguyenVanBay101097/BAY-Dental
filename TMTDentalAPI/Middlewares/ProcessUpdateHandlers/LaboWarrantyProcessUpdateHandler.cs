using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class LaboWarrantyProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.8";
        private IServiceScopeFactory _serviceScopeFactory;


        public LaboWarrantyProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var laboWarrantyRule = context.IRRules.Where(x => x.Code == "labo.labo_warranty_comp_rule").FirstOrDefault();
                if (laboWarrantyRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "LaboWarranty").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Labo Warranty", Model = "LaboWarranty" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "labo.labo_warranty_comp_rule",
                        Name = "Labo Warranty company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
