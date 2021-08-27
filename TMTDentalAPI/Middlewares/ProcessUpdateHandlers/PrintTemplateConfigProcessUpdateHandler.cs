using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class PrintTemplateConfigProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.8";
        private IServiceScopeFactory _serviceScopeFactory;
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;

        public PrintTemplateConfigProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;

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

                var inventoryRule = context.IRRules.Where(x => x.Code == "base.print_template_config_comp_rule").FirstOrDefault();
                if (inventoryRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "PrintTemplateConfig").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Print Template Config", Model = "PrintTemplateConfig" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "base.print_template_config_comp_rule",
                        Name = "Print Template Config multi-company",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                //add data print template 
                //var types = AppConstants.PrintTemplateTypeDemo;
                //foreach (var type in types)
                //{
                //    var printTemplate = context.PrintTemplates.Where(x => x.Type == type.Type).FirstOrDefault();
                //    if (printTemplate == null)
                //    {
                //        printTemplate = new PrintTemplate { Type = type.Type, Content = "" };
                //        context.PrintTemplates.Add(printTemplate);
                //        context.SaveChanges();
                //    }
                //    else
                //    {
                //        printTemplate.Content = "";
                //        context.SaveChanges();
                //    }
                //}

            }

            return Task.CompletedTask;
        }
    }
}
