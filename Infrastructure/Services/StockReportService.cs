﻿using Infrastructure.Data;
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

namespace Infrastructure.Services
{
    public class StockReportService : IStockReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StockReportService(CatalogDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59);
            var date_from = val.DateFrom.HasValue ? val.DateFrom.Value.AbsoluteBeginOfDate() : monthStart;
            var date_to = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : monthEnd;

            var companyId = CompanyId;
            var dict = new Dictionary<Guid, StockReportXuatNhapTonItem>();
            var query = _context.StockHistories.Where(x => x.date >= date_from && x.date <= date_to && x.company_id == companyId);
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
               .GroupBy(x => new
               {
                   ProductId = x.Product.Id,
                   ProductName = x.Product.Name,
                   ProductCode = x.Product.DefaultCode,
                   ProductUomName = x.Product.UOM.Name,
                   MinInventory = x.Product.MinInventory
               })
               .Select(x => new
               {
                   ProductId = x.Key.ProductId,
                   ProductName = x.Key.ProductName,
                   ProductUomName = x.Key.ProductUomName,
                   ProductCode = x.Key.ProductCode,
                   MinInventory = x.Key.MinInventory,
                   Begin = x.Sum(s => s.quantity),
               }).ToListAsync();

            foreach (var item in list)
            {
                if (!dict.ContainsKey(item.ProductId))
                {
                    dict.Add(item.ProductId, new StockReportXuatNhapTonItem()
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductCode = item.ProductCode,
                        ProductUomName = item.ProductUomName,
                        DateFrom = date_from,
                        DateTo = date_to,
                        Begin = item.Begin,
                        MinInventory = item.MinInventory
                    });
                }
            }

            var query2 = _context.StockHistories.Where(x => x.date >= date_from && x.date <= date_to && x.company_id == companyId);
            if (val.ProductCategId.HasValue)
                query2 = query2.Where(x => x.product_categ_id == val.ProductCategId);
            if (val.ProductId.HasValue)
                query2 = query2.Where(x => x.product_id == val.ProductId);
            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query2 = query2.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search) ||
                x.Product.DefaultCode.Contains(val.Search));
            }

            var list2 = await query2.Select(x => new
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductCode = x.Product.DefaultCode,
                ProductUomName = x.Product.UOM.Name,
                MinInventory = x.Product.MinInventory,
                Import = x.quantity > 0 ? x.quantity : 0,
                Export = x.quantity < 0 ? -x.quantity : 0
            })
                      .GroupBy(x => new
                      {
                          ProductId = x.ProductId,
                          ProductName = x.ProductName,
                          ProductCode = x.ProductCode,
                          ProductUomName = x.ProductUomName,
                          MinInventory = x.MinInventory
                      })
                    .Select(x => new
                    {
                        ProductId = x.Key.ProductId,
                        ProductName = x.Key.ProductName,
                        ProductCode = x.Key.ProductCode,
                        productUomName = x.Key.ProductUomName,
                        MinInventory = x.Key.MinInventory,
                        Import = x.Sum(s => s.Import),
                        Export = x.Sum(s => s.Export),
                    }).ToListAsync();

            foreach (var item in list2)
            {
                if (!dict.ContainsKey(item.ProductId))
                {
                    dict.Add(item.ProductId, new StockReportXuatNhapTonItem()
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductCode = item.ProductCode,
                        ProductUomName = item.productUomName,
                        DateFrom = date_from,
                        DateTo = date_to,
                        MinInventory = item.MinInventory,
                        Import = item.Import,
                        Export = item.Export
                    });
                }
                else
                {
                    dict[item.ProductId].Import = item.Import;
                    dict[item.ProductId].Export = item.Export;
                }

                var value = dict[item.ProductId];
                if (value.Begin == 0 && value.Import == 0 && value.Export == 0 && value.End == 0)
                    dict.Remove(item.ProductId);
            }

            foreach (var item in dict.ToArray())
            {
                dict[item.Key].End = dict[item.Key].Begin + dict[item.Key].Import - dict[item.Key].Export;
            }
            if (!string.IsNullOrEmpty(val.MinInventoryFilter) && val.MinInventoryFilter == "above_minInventory")
              return dict.Values.Where(x => x.MinInventory <= x.End);
            if (!string.IsNullOrEmpty(val.MinInventoryFilter) && val.MinInventoryFilter == "below_minInventory")
              return dict.Values.Where(x => x.MinInventory > x.End);

            return dict.Values;
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
