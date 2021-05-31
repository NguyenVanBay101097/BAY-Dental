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
    public class AdvisoryAndQuotationProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public AdvisoryAndQuotationProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                ///Add rule ToothDiagnosis : thông tin chuẩn đoán răng
                var toothDisgnosisRule = context.IRRules.Where(x => x.Code == "tooth.tooth_diagnosis_comp_rule").FirstOrDefault();
                if (toothDisgnosisRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "ToothDiagnosis").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Tooth Diagnosis", Model = "ToothDiagnosis" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "tooth.tooth_diagnosis_comp_rule",
                        Name = "Tooth diagnosis company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }


                ///Add rule Advisory : phiếu tư vấn 
                var advisoryRule = context.IRRules.Where(x => x.Code == "advisory.advisory_comp_rule").FirstOrDefault();
                if (advisoryRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Advisory").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Advisory", Model = "Advisory" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "advisory.advisory_comp_rule",
                        Name = "Advisory company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                ///Add rule quotation : phiếu náo giá
                var quotationRule = context.IRRules.Where(x => x.Code == "quotation.quotation_comp_rule").FirstOrDefault();
                if (quotationRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Quotation").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Quotation", Model = "Quotation" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "quotation.quotation_comp_rule",
                        Name = "Quotation company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
