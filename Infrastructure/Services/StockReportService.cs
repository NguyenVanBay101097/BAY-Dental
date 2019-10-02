using Infrastructure.Data;
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
    public class StockReportService: IStockReportService
    {
        private readonly CatalogDbContext _context;
        public StockReportService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockReportXuatNhapTonItem>> XuatNhapTonSummary(StockReportXuatNhapTonSearch val)
        {
            var today = DateTime.Today;
            var date_from = val.DateFrom.HasValue ? val.DateFrom.Value : new DateTime(today.Year, today.Month, 1);
            var date_to = val.DateTo.HasValue ? val.DateTo.Value.AddDays(1).AddMinutes(-1) : today.AddDays(1).AddMinutes(-1); //23h59
          
            var dict = new Dictionary<Guid, StockReportXuatNhapTonItem>();
            var query = _context.StockHistories.Where(x => x.date < date_from);
            if (val.ProductCategId.HasValue)
                query = query.Where(x => x.product_categ_id == val.ProductCategId);
            if (val.ProductId.HasValue)
                query = query.Where(x => x.product_id == val.ProductId);

            var list = await query
               .GroupBy(x => new {
                   ProductId = x.Product.Id,
                   ProductName = x.Product.Name,
                   ProductCode = x.Product.DefaultCode,
               })
               .Select(x => new
               {
                   ProductId = x.Key.ProductId,
                   ProductName = x.Key.ProductName,
                   ProductCode = x.Key.ProductCode,
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
                        Begin = item.Begin,
                        DateFrom = date_from,
                        DateTo = date_to
                    });
                }
            }

            var query2 = _context.StockHistories.Where(x => x.date >= date_from && x.date <= date_to);
            if (val.ProductCategId.HasValue)
                query2 = query2.Where(x => x.product_categ_id == val.ProductCategId);
            if (val.ProductId.HasValue)
                query2 = query2.Where(x => x.product_id == val.ProductId);

            var list2 = await query2.Select(x => new {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductCode = x.Product.DefaultCode,
                Import = x.quantity > 0 ? x.quantity : 0,
                Export = x.quantity < 0 ? -x.quantity : 0
            })
                      .GroupBy(x => new {
                          ProductId = x.ProductId,
                          ProductName = x.ProductName,
                          ProductCode = x.ProductCode,
                      })
                    .Select(x => new
                    {
                        ProductId = x.Key.ProductId,
                        ProductName = x.Key.ProductName,
                        ProductCode = x.Key.ProductCode,
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
                        DateFrom = date_from,
                        DateTo = date_to
                    });
                }

                dict[item.ProductId].Import = item.Import;
                dict[item.ProductId].Export = item.Export;
                dict[item.ProductId].End = dict[item.ProductId].Begin + dict[item.ProductId].Import - dict[item.ProductId].Export;
            }

            return dict.Values;
        }

        public async Task<IEnumerable<StockReportXuatNhapTonItemDetail>> XuatNhapTonDetail(StockReportXuatNhapTonItem val)
        {
            var today = DateTime.Today;
            var date_from = val.DateFrom.HasValue ? val.DateFrom.Value : new DateTime(today.Year, today.Month, 1);
            var date_to = val.DateTo.HasValue ? val.DateTo.Value : today.AddDays(1).AddMinutes(-1);

            decimal begin = 0;
            var res = new List<AccountCommonPartnerReportItemDetail>();

            begin = await _context.StockHistories.Where(x => x.date < date_from && x.product_id == val.ProductId).SumAsync(x => x.quantity);

            var query2 = _context.StockHistories.Where(x => x.date >= date_from && x.date <= date_to && x.product_id == val.ProductId);
            var list2 = query2.OrderBy(x => x.date)
                    .Select(x => new StockReportXuatNhapTonItemDetail
                    {
                        Date = x.date,
                        MovePickingName = x.Move.Picking.Name,
                        MovePickingId = x.Move.PickingId,
                        MovePickingTypeId = x.Move.Picking.PickingTypeId,
                        Import = x.quantity > 0 ? x.quantity : 0,
                        Export = x.quantity < 0 ? -x.quantity : 0,
                    }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Import - item.Export;
                begin = item.End;
            }

            return list2;
        }
    }
}
