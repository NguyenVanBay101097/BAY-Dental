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

        public async Task<IEnumerable<CommissionProductRuleDisplay>> GetResultAsync(Guid commissionId)
        {
            var productObj = GetService<IProductService>();
            var commissionProductRules = await SearchQuery(x => x.CommissionId == commissionId && x.ProductId.HasValue).Include(x => x.Categ).Include(x => x.Product).ToListAsync();
            var commissionProductRule_dict = commissionProductRules.ToDictionary(x => x.ProductId.Value, x => x);

            var products = await _mapper.ProjectTo<ProductBasic2>(productObj.SearchQuery().Include(x => x.Categ)).ToListAsync();
            var commissionProductRuleList = new List<CommissionProductRuleDisplay>();

            foreach (var product in products)
            {
                if (commissionProductRule_dict.ContainsKey(product.Id))
                {
                    commissionProductRuleList.Add(_mapper.Map<CommissionProductRuleDisplay>(commissionProductRule_dict.ContainsKey(product.Id)));
                }
                else
                {
                    commissionProductRuleList.Add(new CommissionProductRuleDisplay
                    {
                        PercentAdvisory = 0,
                        PercentAssistant = 0,
                        PercentDoctor = 0,
                        ProductId = product.Id,
                        Product = product,
                        CategId = product.CategId,
                        CategName = product.CategName,
                        CompanyId = CompanyId,
                        CommissionId = commissionId
                    });
                }
            }

            return commissionProductRuleList;
        }
    }
}
