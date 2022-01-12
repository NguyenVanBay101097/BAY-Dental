using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelDataService : BaseService<IRModelData>, IIRModelDataService
    {
        private IMyCache _cache;
        private readonly AppTenant _tenant;

        public IRModelDataService(IAsyncRepository<IRModelData> repository, IHttpContextAccessor httpContextAccessor,
            IMyCache cache, ITenant<AppTenant> tenant)
        : base(repository, httpContextAccessor)
        {
            _cache = cache;
            _tenant = tenant?.Value;
        }

        public async Task<IRModelData> GetObjectReference(string reference)
        {
            if (string.IsNullOrEmpty(reference))
                return null;

            var tmp = reference.Split('.');
            var module = tmp[0];
            var name = tmp[1];
            return await SearchQuery(x => x.Name == name && x.Module == module).FirstOrDefaultAsync();
        }

        public async Task<T> GetRef<T>(string reference) where T : class
        {
            var key = $"{(_tenant != null ? _tenant.Hostname : "localhost")}-irmodeldata-{reference}";

            var modelData = await _cache.GetOrCreateAsync<T>(key, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                var data = await GetObjectReference(reference);

                if (data != null && !string.IsNullOrEmpty(data.ResId))
                {
                    switch (data.Model)
                    {
                        case "res.groups":
                            {
                                var service = GetService<IResGroupService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "ir.rule":
                            {
                                var service = GetService<IIRRuleService>();
                                var group = await service.GetByIdAsync(data.ResId);
                                return (T)(object)group;
                            }
                        case "ir.model":
                            {
                                var service = GetService<IIRModelService>();
                                var model = await service.GetByIdAsync(data.ResId);
                                return (T)(object)model;
                            }
                        case "ir.model.access":
                            {
                                var service = GetService<IIRModelAccessService>();
                                var access = await service.GetByIdAsync(data.ResId);
                                return (T)(object)access;
                            }
                        case "res.users":
                            {
                                var service = GetService<UserManager<ApplicationUser>>();
                                var group = await service.FindByIdAsync(data.ResId);
                                return (T)(object)group;
                            }
                        case "ir.module.category":
                            {
                                var service = GetService<IIrModuleCategoryService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "account.account.type":
                            {
                                var service = GetService<IAccountAccountTypeService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "stock.location":
                            {
                                var service = GetService<IStockLocationService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "product.discount":
                            {
                                var service = GetService<IProductService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "account.financial.report":
                            {
                                var service = GetService<IAccountFinancialReportService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "res.partner":
                            {
                                var service = GetService<IPartnerService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "res.sms.campaign":
                            {
                                var service = GetService<ISmsCampaignService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "res.partner.source":
                            {
                                var service = GetService<IPartnerSourceService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "res.partner.title":
                            {
                                var service = GetService<IPartnerTitleService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "res.company":
                            {
                                var service = GetService<ICompanyService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "sample.prescription":
                            {
                                var service = GetService<ISamplePrescriptionService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "print.paper.size":
                            {
                                var service = GetService<IPrintPaperSizeService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "account.financialRevenue.report":
                            {
                                var service = GetService<IAccountFinancialRevenueReportService>();
                                var group = await service.GetByIdAsync(Guid.Parse(data.ResId));
                                return (T)(object)group;
                            }
                        case "print.template":
                            {
                                var template = GetService<IPrintTemplateService>();
                                var printTemplate = await template.GetByIdAsync(data.ResId);
                                return (T)(object)printTemplate;
                            }
                        case "uom":
                            {
                                var service = GetService<IUoMService>();
                                var uom = await service.GetByIdAsync(data.ResId);
                                return (T)(object)uom;
                            }
                        case "uom.category":
                            {
                                var service = GetService<IUoMCategoryService>();
                                var cate = await service.GetByIdAsync(data.ResId);
                                return (T)(object)cate;
                            }
                        case "account.journal":
                            {
                                var service = GetService<IAccountJournalService>();
                                var jounal = await service.GetByIdAsync(data.ResId);
                                return (T)(object)jounal;
                            }
                        case "tooth":
                            {
                                var service = GetService<IToothService>();
                                var jounal = await service.GetByIdAsync(data.ResId);
                                return (T)(object)jounal;
                            }
                        case "stock.picking.type":
                            {
                                var service = GetService<IStockPickingTypeService>();
                                var jounal = await service.GetByIdAsync(data.ResId);
                                return (T)(object)jounal;
                            }
                        case "tooth.category":
                            {
                                var service = GetService<IToothCategoryService>();
                                var toothCate = await service.GetByIdAsync(data.ResId);
                                return (T)(object)toothCate;
                            }
                        case "mail.message.subtype":
                            {
                                var service = GetService<IMailMessageSubtypeService>();
                                var mailMessageSubtype = await service.GetByIdAsync(data.ResId);
                                return (T)(object)mailMessageSubtype;
                            }
                        default:
                            {
                                return null;
                            }
                    }
                }

                return null;
            });

            return modelData;
        }

        public override Task<IEnumerable<IRModelData>> CreateAsync(IEnumerable<IRModelData> entities)
        {
            foreach(var modelData in entities)
            {
                var key = $"{(_tenant != null ? _tenant.Hostname : "localhost")}-irmodeldata-{string.Format("{0}.{1}", modelData.Module, modelData.Name)}";
                _cache.Remove(key);
            }
          
            return base.CreateAsync(entities);
        }

    }
}
