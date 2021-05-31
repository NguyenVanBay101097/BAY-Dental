using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CommissionProductRuleService : BaseService<CommissionProductRule>, ICommissionProductRuleService
    {
        private readonly IMapper _mapper;
        public CommissionProductRuleService(IAsyncRepository<CommissionProductRule> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommissionProductRuleDisplay>> GetForCommission(Guid commissionId)
        {
            var productObj = GetService<IProductService>();
            var commissionProductRules = await SearchQuery(x => x.CommissionId == commissionId && x.ProductId.HasValue && x.CategId.HasValue)
                    .Include(x => x.Categ).Include(x => x.Product).ToListAsync();
            var commissionProductRule_dict = commissionProductRules.ToDictionary(x => x.ProductId.Value, x => x);

            var products = await _mapper.ProjectTo<ProductBasic2>(productObj.SearchQuery(x => x.Type == "service" && x.Type2 == "service" && x.Active).Include(x => x.Categ)).ToListAsync();
            var commissionProductRuleList = new List<CommissionProductRuleDisplay>();

            foreach (var product in products)
            {
                if (commissionProductRule_dict.ContainsKey(product.Id))
                {
                    commissionProductRuleList.Add(_mapper.Map<CommissionProductRuleDisplay>(commissionProductRule_dict[product.Id]));
                }
                else
                {
                    commissionProductRuleList.Add(new CommissionProductRuleDisplay
                    {
                        Percent = 0,
                        ProductId = product.Id,
                        Product = product,
                        CategId = product.CategId,
                        Categ = product.Categ,
                        CompanyId = CompanyId,
                        CommissionId = commissionId,
                        AppliedOn = "0_product_variant"
                    });
                }
            }

            return commissionProductRuleList;
        }

        public async Task CreateUpdateCommissionProductRules(IEnumerable<CommissionProductRuleSave> vals)
        {
            var commissionProductRules_create = new List<CommissionProductRule>();
            var commissionProductRules_update = new List<CommissionProductRule>();
            foreach (var item in vals)
            {
                if (item.Id == Guid.Empty)
                {
                    commissionProductRules_create.Add(_mapper.Map<CommissionProductRule>(item));
                } 
                else
                {
                    commissionProductRules_update.Add(_mapper.Map<CommissionProductRule>(item));
                }
            }
            if (commissionProductRules_create.Any())
            {
                await CreateAsync(commissionProductRules_create);
            }
            if (commissionProductRules_update.Any())
            {
                await UpdateAsync(commissionProductRules_update);

            }
        }
    }
}
