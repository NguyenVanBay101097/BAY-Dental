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
    public class StockInventoryProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.6";
        private IServiceScopeFactory _serviceScopeFactory;
        public StockInventoryProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var inventoryRule = context.IRRules.Where(x => x.Code == "stock.stock_inventory_comp_rule").FirstOrDefault();
                if (inventoryRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "StockInventory").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Stock Inventory", Model = "StockInventory" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }    

                    context.IRRules.Add(new IRRule
                    {
                        Code = "stock.stock_inventory_comp_rule",
                        Name = "Stock inventory company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                var inventoryLineRule = context.IRRules.Where(x => x.Code == "stock.stock_inventory_line_comp_rule").FirstOrDefault();
                if (inventoryLineRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "StockInventoryLine").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Stock Inventory Line", Model = "StockInventoryLine" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "stock.stock_inventory_line_comp_rule",
                        Name = "Stock inventory line company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
