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
    public class IRModelFieldProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.8";
        private IServiceScopeFactory _serviceScopeFactory;


        public IRModelFieldProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                //add standard_price
                var standard_price = context.IRModelFields.Where(x => x.Model == "product.product" && x.TType == "float" && x.Name == "standard_price").FirstOrDefault();
                if (standard_price == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Product").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sản phẩm ", Model = "Product" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRModelFields.Add(new IRModelField
                    {
                        IRModelId = model.Id,
                        Model = "product.product",
                        Name = "standard_price",
                        TType = "float",
                    });

                    context.SaveChanges();
                }

                //add member_level
                var member_level = context.IRModelFields.Where(x => x.Model == "res.partner" && x.TType == "many2one" && x.Name == "member_level" && x.Relation == "member.level").FirstOrDefault();
                if (member_level == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Partner").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Đối tác ", Model = "Partner" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRModelFields.Add(new IRModelField
                    {
                        IRModelId = model.Id,
                        Model = "res.partner",
                        Name = "member_level",
                        TType = "many2one",
                        Relation = "member.level"
                    });

                    context.SaveChanges();
                }

                //add loyalty_points
                var loyalty_points = context.IRModelFields.Where(x => x.Model == "res.partner" && x.TType == "float" && x.Name == "loyalty_points").FirstOrDefault();
                if (loyalty_points == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Partner").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Đối tác ", Model = "Partner" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRModelFields.Add( new IRModelField
                    {
                        IRModelId = model.Id,
                        Model = "res.partner",
                        Name = "loyalty_points",
                        TType = "float",
                    });

                    context.SaveChanges();
                }


              

            }

            return Task.CompletedTask;
        }
    }
}
