using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ApplicationCore.Utilities;
using ApplicationCore.Models;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using System.Dynamic;

namespace Infrastructure.Services
{
    public class StockReportService : IStockReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAsyncRepository<Product> _productRepository;
        public StockReportService(CatalogDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAsyncRepository<Product> productRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        public async Task<IEnumerable<StockReportXuatNhapTonItem>> XuatNhapTonSummary(StockReportXuatNhapTonSearch val)
        {
            var today = DateTime.Today;
            var date_from = val.DateFrom.HasValue ? val.DateFrom.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var date_to = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;

            var companyId = CompanyId;
            var dict = new Dictionary<Guid, dynamic>();
            if (date_from.HasValue)
            {
                var query = _context.StockHistories.Where(x => x.date < date_from && x.company_id == companyId);
                if (val.ProductCategId.HasValue)
                    query = query.Where(x => x.product_categ_id == val.ProductCategId);
                if (val.ProductId.HasValue)
                    query = query.Where(x => x.product_id == val.ProductId);
                if (!string.IsNullOrWhiteSpace(val.Search))
                {
                    query = query.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search) ||
                    x.Product.DefaultCode.Contains(val.Search));
                }

                var list = await query
                   .GroupBy(x => x.product_id)
                   .Select(x => new
                   {
                       ProductId = x.Key,
                       Begin = x.Sum(s => s.quantity),
                   }).ToListAsync();

                foreach (var item in list)
                {
                    if (item.ProductId.HasValue && !dict.ContainsKey(item.ProductId.Value))
                    {
                        dynamic exo = new ExpandoObject();
                        exo.Begin = item.Begin;
                        exo.Import = 0;
                        exo.Export = 0;

                        dict.Add(item.ProductId.Value, exo);
                    }
                }
            }
           

            var query2 = _context.StockHistories.Where(x => (!date_from.HasValue || x.date >= date_from) && (!date_to.HasValue || x.date <= date_to) && x.company_id == companyId);
            if (val.ProductCategId.HasValue)
                query2 = query2.Where(x => x.product_categ_id == val.ProductCategId);
            if (val.ProductId.HasValue)
                query2 = query2.Where(x => x.product_id == val.ProductId);
            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query2 = query2.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search) ||
                x.Product.DefaultCode.Contains(val.Search));
            }

            var list2 = await query2.GroupBy(x => x.product_id)
                    .Select(x => new
                    {
                        ProductId = x.Key,
                        Import = x.Sum(s => s.quantity > 0 ? s.quantity : 0),
                        Export = x.Sum(s => s.quantity < 0 ? -s.quantity : 0),
                    }).ToListAsync();

            foreach (var item in list2)
            {
                if (item.ProductId.HasValue && !dict.ContainsKey(item.ProductId.Value))
                {
                    dynamic exo = new ExpandoObject();
                    exo.Begin = 0;
                    exo.Import = item.Import;
                    exo.Export = item.Export;

                    dict.Add(item.ProductId.Value, exo);
                }
                else
                {
                    var exo = dict[item.ProductId.Value];
                    exo.Import = item.Import;
                    exo.Export = item.Export;
                }
            }

            var productIds = dict.Keys.ToArray();
            var products = await _productRepository.SearchQuery(x => productIds.Contains(x.Id))
                .Include(x => x.UOM).ToListAsync();
            var productDict = products.ToDictionary(x => x.Id, x => x);

            var result = new List<StockReportXuatNhapTonItem>();
            foreach (var item in dict)
            {
                var data = item.Value;
                if (data.Begin == 0 && data.Import == 0 && data.Export == 0)
                    continue;
                var product = productDict[item.Key];
                result.Add(new StockReportXuatNhapTonItem
                {
                    Begin = data.Begin,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo,
                    End = data.Begin + data.Import - data.Export,
                    Export = data.Export,
                    Import = data.Import,
                    ProductCode = product.DefaultCode,
                    MinInventory = product.MinInventory ?? 0,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductUomName = product.UOM.Name,
                    ProductNameNoSign = product.NameNoSign
                });
            }
           
            return result;
        }

        public async Task<IEnumerable<StockReportXuatNhapTonItemDetail>> XuatNhapTonDetail(StockReportXuatNhapTonItem val)
        {
            var today = DateTime.Today;
            var date_from = val.DateFrom.HasValue ? val.DateFrom.Value : new DateTime(today.Year, today.Month, 1);
            var date_to = val.DateTo.HasValue ? val.DateTo.Value : today.AddDays(1).AddMinutes(-1);
            var companyId = CompanyId;
            decimal begin = 0;
            var res = new List<AccountCommonPartnerReportItemDetail>();

            begin = await _context.StockHistories.Where(x => x.date < date_from && x.product_id == val.ProductId && x.company_id == companyId).SumAsync(x => x.quantity);

            var query2 = _context.StockHistories.Where(x => x.date >= date_from && x.date <= date_to && x.product_id == val.ProductId && x.company_id == companyId);
            var list2 = query2.GroupBy(x => new { MoveId = x.move_id, Date = x.date, PickingName = x.Move.Picking.Name, PickingId = x.Move.PickingId }).Select(x => new
            {
                Date = x.Key.Date,
                MoveId = x.Key.MoveId,
                Quantity = x.Sum(s => s.quantity),
                PickingName = x.Key.PickingName,
                PickingId = x.Key.PickingId
            }).OrderBy(x => x.Date).Select(x => new StockReportXuatNhapTonItemDetail
            {
                Date = x.Date,
                MovePickingName = x.PickingName,
                MovePickingId = x.PickingId,
                Import = x.Quantity > 0 ? x.Quantity : 0,
                Export = x.Quantity < 0 ? -x.Quantity : 0,
            }).ToList();
            //var list2 = query2.OrderBy(x => x.date)
            //        .Select(x => new StockReportXuatNhapTonItemDetail
            //        {
            //            Date = x.date,
            //            MovePickingName = x.Move.Picking.Name,
            //            MovePickingId = x.Move.PickingId,
            //            MovePickingTypeId = x.Move.Picking.PickingTypeId,
            //            Import = x.quantity > 0 ? x.quantity : 0,
            //            Export = x.quantity < 0 ? -x.quantity : 0,
            //            PriceUnitOnQuant = x.price_unit_on_quant
            //        }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Import - item.Export;
                begin = item.End;
            }

            return list2;
        }

        public async Task<PagedResult2<GetStockHistoryRes>> GetStockHistoryPaged(GetStockHistoryReq val)
        {
            var query = _context.StockHistories.AsQueryable();
            if (val.DateFrom.HasValue)
            {
                query = query.Where(x => x.date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            }
            if (val.DateTo.HasValue)
            {
                query = query.Where(x => x.date <= val.DateTo.Value.AbsoluteEndOfDate());
            }
            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.product_id == val.ProductId);
            }

            var count = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var res = await query.Select(x => new GetStockHistoryRes()
            {
                Date = x.date,
                MovePickingName = x.Move.Picking.Name,
                Quantity = x.quantity
            }).ToListAsync();

            return new PagedResult2<GetStockHistoryRes>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }
    }
}
