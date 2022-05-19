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
    public class IRModelDataToothCategoryProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {

        private const string _version = "1.0.1.6";
        private IServiceScopeFactory _serviceScopeFactory;
        public IRModelDataToothCategoryProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                //var tenant = scope.ServiceProvider.GetService<AppTenant>();
                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var adultCategory = context.IRModelDatas.Where(x => x.Name.Equals("tooth_category_adult") 
                && x.Model.Equals("tooth.category") && x.Module.Equals("base")).FirstOrDefault();
                if (adultCategory == null)
                {
                    var adultId = context.ToothCategories.Where(x => x.Name.Equals("Răng vĩnh viễn")).Select(x => x.Id).FirstOrDefault();
                    if (adultId != null)
                    {
                        adultCategory = new IRModelData()
                        {
                            Name = "tooth_category_adult",
                            Model = "tooth.category",
                            Module = "base",
                            ResId = adultId.ToString()
                        };
                        context.IRModelDatas.Add(adultCategory);
                    }
                }

                var childCategory = context.IRModelDatas.Where(x => x.Name.Equals("tooth_category_child")
                && x.Model.Equals("tooth.category") && x.Module.Equals("base")).FirstOrDefault();
                if (childCategory == null)
                {
                    var childId = context.ToothCategories.Where(x => x.Name.Equals("Răng sữa")).Select(x => x.Id).FirstOrDefault();
                    if (childId != null)
                    {
                        childCategory = new IRModelData()
                        {
                            Name = "tooth_category_child",
                            Model = "tooth.category",
                            Module = "base",
                            ResId = childId.ToString()
                        };
                        context.IRModelDatas.Add(childCategory);
                    }
                }
                context.SaveChanges();

            }

            return Task.CompletedTask;
        }
    }
}
