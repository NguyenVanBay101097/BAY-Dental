using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
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
    public class SaleReportService : ISaleReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public SaleReportService(CatalogDbContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<SaleReportTopServicesCs>> GetTopServices(SaleReportTopServicesFilter val)
        {
            var query = _context.SaleReports.AsQueryable();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId.Equals(val.PartnerId));
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId.Equals(val.CompanyId));
            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId.Equals(val.CategId));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State.ToLower()));
            if(val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo);

            if (val.ByInvoice)
            {
                return await query.GroupBy(x => new
                            {
                                ProductId = x.ProductId,
                                ProductName = x.Product.Name
                            }).Select(x => new SaleReportTopServicesCs
                            {
                                ProductId = x.Key.ProductId ?? Guid.Empty,
                                PriceTotalSum = x.Sum(y => y.PriceTotal),
                                ProductUOMQtySum = x.Sum(y => y.ProductUOMQty),
                                ProductName = x.Key.ProductName
                            }).OrderByDescending(x => x.PriceTotalSum).ThenByDescending(x => x.ProductUOMQtySum).Take(val.Number)
                            .ToListAsync();
            } else
            {
                return await query.GroupBy(x => new
                            {
                                ProductId = x.ProductId,
                                ProductName = x.Product.Name
                            }).Select(x => new SaleReportTopServicesCs
                            {
                                ProductId = x.Key.ProductId ?? Guid.Empty,
                                ProductUOMQtySum = x.Sum(y => y.ProductUOMQty),
                                PriceTotalSum = x.Sum(y => y.PriceTotal),
                                ProductName = x.Key.ProductName
                            }).OrderByDescending(x => x.ProductUOMQtySum).ThenByDescending(x=>x.PriceTotalSum).Take(val.Number)
                            .ToListAsync();
            }
        }
    }
}
