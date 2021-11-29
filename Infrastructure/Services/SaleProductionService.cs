using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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
    public class SaleProductionService : BaseService<SaleProduction>, ISaleProductionService
    {
        private readonly IMapper _mapper;

        public SaleProductionService(IAsyncRepository<SaleProduction> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task ChangeQtySaleProduction(SaleProductionChangeQtyReq val)
        {

        }

        public async Task CompareSaleProduction(IEnumerable<SaleProduction> saleProductions)
        {
           
            var removeItems = new List<SaleProduction>();
            var updateItems = new List<SaleProduction>();
            var createItems = new List<SaleProduction>();

            foreach (var saleProduction in saleProductions)
            {
                if (!saleProduction.SaleOrderLineRels.Any())
                    removeItems.Add(saleProduction);

                if (saleProduction.Id == Guid.Empty)
                {
                    createItems.Add(saleProduction);
                }
                else
                {
                    var production = saleProductions.FirstOrDefault(x => x.Id == saleProduction.Id);
                    var newQty = production.SaleOrderLineRels.Sum(x => x.OrderLine.ProductUOMQty);

                    foreach (var line in production.Lines)
                    {
                        var qty = line.Quantity / saleProduction.Quantity * newQty;
                        line.Quantity = qty;
                    }

                    production.Quantity = newQty;

                    updateItems.Add(saleProduction);
                }
               
            }

            if (createItems.Any())
                await CreateAsync(createItems);

            if (updateItems.Any())
                await UpdateAsync(updateItems);

            if (removeItems.Any())
                await DeleteAsync(removeItems);
          
        }


        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var saleProductions = await SearchQuery(x => ids.Contains(x.Id))
              .Include(x => x.SaleOrderLineRels)
              .ToListAsync();

            await DeleteAsync(saleProductions);
        }


        public override ISpecification<SaleProduction> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "sale.sale_production_comp_rule":
                    return new InitialSpecification<SaleProduction>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
