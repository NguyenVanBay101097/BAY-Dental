using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelDataService : BaseService<IRModelData>, IIRModelDataService
    {
        public IRModelDataService(IAsyncRepository<IRModelData> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
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
                    default:
                        {
                            return null;
                        }
                }
            }

            return null;
        }
    }
}
