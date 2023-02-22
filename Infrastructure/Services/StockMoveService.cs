﻿using ApplicationCore.Entities;
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
    public class StockMoveService : BaseService<StockMove>, IStockMoveService
    {
        private readonly IMapper _mapper;

        public StockMoveService(IAsyncRepository<StockMove> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task ActionDone(IEnumerable<StockMove> self)
        {
            var productObj = GetService<IProductService>();
            foreach (var move in self)
            {
                if (!move.PriceUnit.HasValue)
                    move.PriceUnit = productObj.GetStandardPrice(move.ProductId, force_company_id: move.CompanyId);
            }

            await UpdateAsync(self);

            await ProductPriceUpdateBeforeDone(self);

            foreach (var move in self)
            {
                if (!move.PriceUnit.HasValue)
                    move.PriceUnit = move.Product.StandardPrice(CompanyId);

                if (move.ProductUOMQty <= 0)
                    continue;
                var quantObj = GetService<IStockQuantService>();
                var quants = await quantObj.QuantsGetReservation(move.ProductQty ?? 0, move);
                await quantObj.QuantsMove(quants, move, move.LocationDest);
                move.State = "done";
            }

            await UpdateAsync(self);
        }

        public async Task ProductPriceUpdateBeforeDone(IEnumerable<StockMove> self)
        {
            var productObj = GetService<IProductService>();
            var tmpl_dict = new Dictionary<Guid, decimal>();
            var std_price_update = new Dictionary<Guid, decimal>();
            //var usages = new string[] { "supplier", "production" };
            //var moves = self.Where(x => usages.Contains(x.Location.Usage));
            //if (!moves.Any())
            //    return;

            //var companyId = CompanyId;
            //if (!self.All(x => x.CompanyId == companyId))
            //    throw new Exception("Chi tiết xuất nhập kho phải thuộc chi nhánh bạn đang làm việc");

            var productIds = self.Select(x => x.ProductId).Distinct().ToList();
            var qtyAvailableDict = await productObj.GetQtyAvailableDict(productIds);
            var products = await productObj.SearchQuery(x => productIds.Contains(x.Id)).Include(x => x.ProductCompanyRels).ToListAsync();
            var productDict = products.ToDictionary(x => x.Id, x => x);

            foreach (var move in self.Where(x => x.IsIn()))
            {
                if (!tmpl_dict.ContainsKey(move.Product.Id))
                    tmpl_dict.Add(move.Product.Id, 0);
                var product = productDict[move.ProductId];
                if (!std_price_update.ContainsKey(move.ProductId))
                {
                    var standardPrice = productObj.GetStandardPrice(move.ProductId, force_company_id: move.CompanyId);
                    std_price_update.Add(move.ProductId, (decimal)standardPrice);
                }

                var product_tot_qty_available = qtyAvailableDict[move.ProductId] + tmpl_dict[move.Product.Id];
                decimal new_std_price = 0;
                var move_price_unit = _GetPriceUnit(move);
                var move_product_qty = move.ProductQty ?? 0;
                if (product_tot_qty_available <= 0)
                {
                    new_std_price = move_price_unit;
                }
                else
                {
                    //Get the standard price
                    var amount_unit = std_price_update[move.ProductId];
                    new_std_price = ((amount_unit * product_tot_qty_available) + (move_price_unit * move_product_qty)) /
                        (product_tot_qty_available + move_product_qty);
                }

                tmpl_dict[move.Product.Id] += move_product_qty;
                std_price_update[move.ProductId] = new_std_price;
                // update standprice and add product price history
                productObj.SetStandardPrice(product, (double)Math.Round(new_std_price, 2), move.CompanyId);
            }

            //foreach (var item in std_price_update)
            //{
            //    var product = productDict[item.Key];
            //    product.SetStandardPrice((double)item.Value, companyId);
            //    product.PriceHistories.Add(new ProductPriceHistory { Cost = (double)item.Value, CompanyId = companyId });
            //}
            //await productObj.UpdateAsync(productDict.Values);
        }

        public decimal _GetPriceUnit(StockMove self)
        {
            return (decimal)self.PriceUnit;
        }

        public async Task<StockMoveOnChangeProductResult> OnChangeProduct(StockMoveOnChangeProduct val)
        {
            var res = new StockMoveOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId).Include(x => x.UOM).FirstOrDefaultAsync();
                res.Name = product.Name;
                res.ProductUOM = _mapper.Map<UoMBasic>(product.UOM);
            }

            return res;
        }

        public override ISpecification<StockMove> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "stock.stock_move_rule":
                    return new InitialSpecification<StockMove>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public void _Compute(IEnumerable<StockMove> self)
        {
            _ComputeProductQty(self);
        }

        private void _ComputeProductQty(IEnumerable<StockMove> moves)
        {
            var uomObj = GetService<IUoMService>();
            var productObj = GetService<IProductService>();
            foreach (var move in moves)
            {
                if (move.ProductUOM == null || move.ProductUOM.Id != move.ProductUOMId)
                    move.ProductUOM = uomObj.GetById(move.ProductUOMId);

                if (move.Product == null || move.Product.Id != move.ProductId || move.Product.UOM == null)
                    move.Product = productObj.SearchQuery(x => x.Id == move.ProductId).Include(x => x.UOM).FirstOrDefault();

                move.ProductQty = uomObj.ComputeQtyObj(move.ProductUOM, move.ProductUOMQty, move.Product.UOM);
            }

        }


        public override Task<IEnumerable<StockMove>> CreateAsync(IEnumerable<StockMove> entities)
        {
            _Compute(entities);
            return base.CreateAsync(entities);
        }
    }

    public class StdPriceUpdateKey
    {
        public Guid CompanyId { get; set; }
        public Guid ProductId { get; set; }
    }
}
