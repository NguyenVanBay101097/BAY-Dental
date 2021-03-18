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
    public class SurveyProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.6";
        private IServiceScopeFactory _serviceScopeFactory;
        public SurveyProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var categ = context.IrModuleCategories.Where(x => x.Name == "Survey").FirstOrDefault();
                if (categ == null)
                {
                    categ = new IrModuleCategory()
                    {
                        Name = "Survey",
                        Visible = false
                    };
                    context.IrModuleCategories.Add(categ);
                    context.SaveChanges();

                    var surveyAssignmentModel = new IRModel()
                    {
                        Name = "Survey Assignment",
                        Model = "SurveyAssignment",
                        Transient = true,
                    };
                    context.IRModels.Add(surveyAssignmentModel);
                    context.SaveChanges();

                    var surveyAssignmentRule = new IRRule()
                    {
                        Name = "Survey Assignment Employee Rule",
                        ModelId = surveyAssignmentModel.Id,
                        Code = "survey.survey_assignment_employee_rule",
                        Global = false
                    };

                    context.IRRules.Add(surveyAssignmentRule);
                    context.SaveChanges();

                    var empGroup = new ResGroup()
                    {
                        CategoryId = categ.Id,
                        Name = "Nhân viên",
                    };
                    empGroup.RuleGroupRels.Add(new RuleGroupRel { RuleId = surveyAssignmentRule.Id });

                    var manageGroup = new ResGroup()
                    {
                        CategoryId = categ.Id,
                        Name = "Quản lý",
                    };

                    context.ResGroups.AddRange(new List<ResGroup>() { empGroup, manageGroup });
                    context.SaveChanges();

                    context.IRModelDatas.Add(new IRModelData() // for show combobox
                    {
                        Name = "module_category_survey",
                        Module = "survey",
                        ResId = categ.Id.ToString(),
                        Model = "ir.module.category"
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
