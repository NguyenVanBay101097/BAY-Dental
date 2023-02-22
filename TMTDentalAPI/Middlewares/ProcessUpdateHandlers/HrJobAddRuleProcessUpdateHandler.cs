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
    public class HrJobAddRuleProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.0";
        private IServiceScopeFactory _serviceScopeFactory;
        public HrJobAddRuleProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var rule = context.IRRules.Where(x => x.Code == "hr.hr_job_comp_rule").FirstOrDefault();
                if (rule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "HrJob").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Hr Job", Model = "HrJob" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "hr.hr_job_comp_rule",
                        Name = "Job multi company rule",
                        ModelId = model.Id
                    });

                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
