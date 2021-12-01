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

        public async Task UpdateSaleProduction(UpdateSaleProductionReq val)
        {
            var updateItems = new List<SaleProduction>();
            var createItems = new List<SaleProduction>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var bomObj = GetService<IProductBomService>();
            var states = new string[] { "draft", "cancel" };
            var lines = await orderLineObj.SearchQuery(x => x.OrderId == val.OrderId && x.Product.Boms.Any() && !states.Contains(x.State)).Include(x => x.SaleProductionRels).ThenInclude(s => s.SaleProduction).ToListAsync();
            var items = lines.GroupBy(x => x.ProductId.Value).Select(s => new
            {
                OrderLineIds = s.Select(x => x.Id).ToList(),
                ProductId = s.Key,
                Qty = s.Sum(x => x.ProductUOMQty)
            }).ToList();


            var saleProduction_dict = await SearchQuery(x => x.SaleOrderLineRels.Any(s => lines.Select(x => x.Id).Contains(s.OrderLineId))).Include(x => x.Lines).Include(x => x.SaleOrderLineRels).ToDictionaryAsync(x => x.ProductId.Value, x => x);
            var boms = await bomObj.SearchQuery(x => items.Distinct().Select(x => x.ProductId).Contains(x.ProductId)).ToListAsync();
            var bom_dict = boms.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var item in items)
            {
                if (!saleProduction_dict.ContainsKey(item.ProductId))
                {
                    var saleProduction = new SaleProduction
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Qty,
                        CompanyId = CompanyId
                    };

                    foreach (var line in bom_dict[item.ProductId])
                    {
                        saleProduction.Lines.Add(new SaleProductionLine
                        {
                            ProductId = line.MaterialProductId.Value,
                            Quantity = saleProduction.Quantity * line.Quantity
                        });
                    }

                    foreach (var lineId in item.OrderLineIds)
                    {
                        saleProduction.SaleOrderLineRels.Add(new SaleOrderLineSaleProductionRel { OrderLineId = lineId });
                    }

                    createItems.Add(saleProduction);
                }
                else
                {
                    var saleProduction = saleProduction_dict[item.ProductId];

                    saleProduction.Quantity = item.Qty;

                    foreach (var line in bom_dict[item.ProductId])
                    {
                        var itemLine = saleProduction.Lines.Where(x => x.ProductId == line.MaterialProductId).FirstOrDefault();
                        if (itemLine != null)
                        {
                            itemLine.Quantity = line.Quantity * saleProduction.Quantity;
                        }
                        else
                        {
                            saleProduction.Lines.Add(new SaleProductionLine
                            {
                                ProductId = line.MaterialProductId.Value,
                                Quantity = saleProduction.Quantity * line.Quantity
                            });
                        }
                    }

                    foreach (var lineId in item.OrderLineIds)
                    {
                        if (saleProduction.SaleOrderLineRels.Any(x => x.OrderLineId == lineId))
                            continue;

                        saleProduction.SaleOrderLineRels.Add(new SaleOrderLineSaleProductionRel { OrderLineId = lineId });
                    }

                    updateItems.Add(saleProduction);
                }
            }


            if (createItems.Any())
                await CreateAsync(createItems);

            if (updateItems.Any())
                await UpdateAsync(updateItems);

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


            if (updateItems.Any())
                await UpdateAsync(updateItems);

            if (removeItems.Any())
                await DeleteAsync(removeItems);

        }


        public async Task Unlink(IEnumerable<Guid> ids)
        {
            //nếu có yêu cầu vật tư nào đang yêu cầu hoặc đã xuất thì ko đc xóa
            var productRequestLineObj = GetService<IProductRequestLineService>();
            var requestLines = await productRequestLineObj.SearchQuery(x => x.SaleProductionLineRels.Any(s => ids.Contains(s.SaleProductionLine.SaleProductionId)))
                .Include(x => x.Request)
                .ToListAsync();

            if (requestLines.Any(x => x.Request.State != "draft"))
                throw new Exception("Có yêu cầu vật tư đang yêu cầu hoặc đã xuất, không thể xóa");

            await productRequestLineObj.DeleteAsync(requestLines);

            var saleProductions = await SearchQuery(x => ids.Contains(x.Id))
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
