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
    public class PartnerAdvanceProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public PartnerAdvanceProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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
                    Version version1 = new Version(_version);
                    Version version2 = new Version(tenant.Version);
                    if (version2.CompareTo(version1) >= 0)
                        return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var partnerAdvanceRule = context.IRRules.Where(x => x.Code == "partner.partner_advance_comp_rule").FirstOrDefault();
                if (partnerAdvanceRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "PartnerAdvance").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Partner Advance", Model = "PartnerAdvance" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "partner.partner_advance_comp_rule",
                        Name = "Partner advance company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

              
            }

            return Task.CompletedTask;
        }
    }
}
