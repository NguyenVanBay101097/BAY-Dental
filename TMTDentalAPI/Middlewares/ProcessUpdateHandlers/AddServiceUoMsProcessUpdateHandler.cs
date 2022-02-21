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
    public class AddServiceUoMsProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.4";
        private IServiceScopeFactory _serviceScopeFactory;


        public AddServiceUoMsProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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
                UoMCategory uomCategory;
                var modelData = context.IRModelDatas.Where(x => x.Name == "product_uom_categ_service" && x.Module == "product").FirstOrDefault();
                if (modelData == null)
                {
                    uomCategory = new UoMCategory { Name = "Dịch vụ", MeasureType = "unit" };
                    context.UoMCategories.Add(uomCategory);
                    context.SaveChanges();

                    modelData = new IRModelData
                    {
                        Module = "product",
                        Name = "product_uom_categ_service",
                        Model = "uom.category",
                        ResId = uomCategory.Id.ToString()
                    };
                    context.IRModelDatas.Add(modelData);
                    context.SaveChanges();

                    var uoms = new List<UoM>()
                    {
                        new UoM { Name = "Răng", CategoryId = uomCategory.Id },
                        new UoM { Name = "Từng hàm", CategoryId = uomCategory.Id },
                        new UoM { Name = "Nguyên hàm", CategoryId = uomCategory.Id },
                        new UoM { Name = "Gói", CategoryId = uomCategory.Id },
                        new UoM { Name = "Lần", CategoryId = uomCategory.Id },
                        new UoM { Name = "Liệu trình", CategoryId = uomCategory.Id },
                        new UoM { Name = "Trụ", CategoryId = uomCategory.Id },
                        new UoM { Name = "Vít", CategoryId = uomCategory.Id },
                        new UoM { Name = "Trọn gói", CategoryId = uomCategory.Id },
                        new UoM { Name = "Tuýp", CategoryId = uomCategory.Id },
                        new UoM { Name = "Ống tủy", CategoryId = uomCategory.Id },
                        new UoM { Name = "Nang", CategoryId = uomCategory.Id },
                        new UoM { Name = "Áp xe", CategoryId = uomCategory.Id },
                        new UoM { Name = "Phim", CategoryId = uomCategory.Id },
                        new UoM { Name = "Xoang", CategoryId = uomCategory.Id },
                        new UoM { Name = "Cái", CategoryId = uomCategory.Id },
                        new UoM { Name = "Bộ", CategoryId = uomCategory.Id }
                    };

                    context.UoMs.AddRange(uoms);
                    context.SaveChanges();
                }    
            }

            return Task.CompletedTask;
        }
    }
}

