using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class ProductRequestProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.6";
        private IServiceScopeFactory _serviceScopeFactory;
        public ProductRequestProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var inventoryRule = context.IRRules.Where(x => x.Code == "productrequest.product_request_comp_rule").FirstOrDefault();
                if (inventoryRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "ProductRequest").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Product Request", Model = "ProductRequest" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "productrequest.product_request_comp_rule",
                        Name = "Product Request company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
