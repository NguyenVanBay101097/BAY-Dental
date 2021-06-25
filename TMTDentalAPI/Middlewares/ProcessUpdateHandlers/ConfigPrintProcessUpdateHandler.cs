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
    public class ConfigPrintProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public ConfigPrintProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var inventoryRule = context.IRRules.Where(x => x.Code == "config.config_print_comp_rule").FirstOrDefault();
                if (inventoryRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "ConfigPrint").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Config Print", Model = "ConfigPrint" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "config.config_print_comp_rule",
                        Name = "Config Print multi-company",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                //add sample data print paper size
                var irmodelData = context.IRModelDatas.Where(x => x.Model == "print.paper.size").ToList();
                if (!irmodelData.Any())
                {
                    var papersize_A4 = new PrintPaperSize { Name = "A4", PaperFormat = "A4", TopMargin = 10, BottomMargin = 10, LeftMargin = 10, RightMargin = 10 };
                    context.PrintPaperSizes.Add(papersize_A4);

                    var iRmodelData_A4 = context.IRModelDatas.Where(x => x.Name == "paperformat_a4" && x.Module == "base").FirstOrDefault();
                    if(iRmodelData_A4 == null)
                    {
                        iRmodelData_A4 = new IRModelData { Name = "paperformat_a4", Module = "base" , ResId = papersize_A4.Id.ToString() , Model = "print.paper.size" };
                        context.IRModelDatas.Add(iRmodelData_A4);
                        context.SaveChanges();
                    }

                    var papersize_A5 = new PrintPaperSize { Name = "A5", PaperFormat = "A5" , TopMargin = 5 , BottomMargin = 5, LeftMargin = 5, RightMargin = 5 };
                    context.PrintPaperSizes.Add(papersize_A5);

                    var iRmodelData_A5 = context.IRModelDatas.Where(x => x.Name == "paperformat_a5" && x.Module == "base").FirstOrDefault();
                    if (iRmodelData_A5 == null)
                    {
                        iRmodelData_A5 = new IRModelData { Name = "paperformat_a5", Module = "base", ResId = papersize_A5.Id.ToString(), Model = "print.paper.size" };
                        context.IRModelDatas.Add(iRmodelData_A5);
                        context.SaveChanges();
                    }
                }              
               
            }

            return Task.CompletedTask;
        }
    }
}
