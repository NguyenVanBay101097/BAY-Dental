﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class AdvisoryService : BaseService<Advisory>, IAdvisoryService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public AdvisoryService(
            IAsyncRepository<Advisory> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IUserService userService
        ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<PagedResult2<AdvisoryBasic>> GetPagedResultAsync(AdvisoryPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.User.Name.Contains(val.Search) ||
                    x.AdvisoryProductRels.Any(s => s.Product.Name.Contains(val.Search)));
            }

            if (val.ToothIds.Any())
            {
                query = query.Where(x => x.AdvisoryToothRels.Any(s => val.ToothIds.Contains(s.ToothId)));
            }

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == val.CustomerId);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId);
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.User)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<AdvisoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AdvisoryBasic>>(items)
            };

            return paged;
        }

        public async Task<AdvisoryDisplay> GetAdvisoryDisplay(Guid id)
        {
            var advisory = await SearchQuery(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Customer)
                .Include(x => x.ToothCategory)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();

            var res = _mapper.Map<AdvisoryDisplay>(advisory);

            return res;
        }

        public async Task<Advisory> CreateAdvisory(AdvisorySave val)
        {
            var advisory = _mapper.Map<Advisory>(val);

            // Thêm răng
            if (val.ToothIds.Any())
            {
                foreach (var toothId in val.ToothIds)
                {
                    advisory.AdvisoryToothRels.Add(new AdvisoryToothRel() { ToothId = toothId });
                }
            }
            // Thêm chuẩn đoán răng
            if (val.ToothDiagnosisIds.Any())
            {
                foreach (var toothDiagnosisId in val.ToothDiagnosisIds)
                {
                    advisory.AdvisoryToothDiagnosisRels.Add(new AdvisoryToothDiagnosisRel() { ToothDiagnosisId = toothDiagnosisId });
                }
            }
            // Thêm dịch vụ tư vấn
            if (val.ProductIds.Any())
            {
                foreach (var productId in val.ProductIds)
                {
                    advisory.AdvisoryProductRels.Add(new AdvisoryProductRel() { ProductId = productId });
                }
            }

            await CreateAsync(advisory);

            return advisory;
        }

        public async Task UpdateAdvisory(Guid id, AdvisorySave val)
        {
            var advisory = await SearchQuery(x => x.Id == id)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();
            advisory = _mapper.Map(val, advisory);

            // Xóa răng
            advisory.AdvisoryToothRels.Clear();
            // Thêm răng
            if (val.ToothIds.Any())
            {
                foreach (var toothId in val.ToothIds)
                {
                    advisory.AdvisoryToothRels.Add(new AdvisoryToothRel() { ToothId = toothId });
                }
            }

            // Xóa chuẩn đoán răng
            advisory.AdvisoryToothDiagnosisRels.Clear();
            // Thêm chuẩn đoán răng
            if (val.ToothDiagnosisIds.Any())
            {
                foreach (var toothDiagnosisId in val.ToothDiagnosisIds)
                {
                    advisory.AdvisoryToothDiagnosisRels.Add(new AdvisoryToothDiagnosisRel() { ToothDiagnosisId = toothDiagnosisId });
                }
            }

            // Xóa dịch vụ tư vấn
            advisory.AdvisoryProductRels.Clear();
            // Thêm dịch vụ tư vấn
            if (val.ProductIds.Any())
            {
                foreach (var productId in val.ProductIds)
                {
                    advisory.AdvisoryProductRels.Add(new AdvisoryProductRel() { ProductId = productId });
                }
            }

            await UpdateAsync(advisory);
        }

        public async Task RemoveAdvisory(Guid id)
        {
            var advisory = await SearchQuery(x => x.Id == id)
                .Include(x => x.AdvisoryToothRels)
                .Include(x => x.AdvisoryToothDiagnosisRels)
                .Include(x => x.AdvisoryProductRels)
                .FirstOrDefaultAsync();
            await DeleteAsync(advisory);
        }

        public async Task<AdvisoryDisplay> DefaultGet(AdvisoryDefaultGet val)
        {
            var user = await _userService.GetCurrentUser();
            var res = new AdvisoryDisplay();
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            res.UserId = res.User.Id;
            res.CompanyId = CompanyId;
            res.Date = DateTime.Now;
            if (val.CustomerId.HasValue)
            {
                // Lấy thông tin khách hàng
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(val.CustomerId);
                res.CustomerId = partner.Id;
                res.Customer = _mapper.Map<PartnerSimple>(partner);
            }

            return res;
        }

        public async Task<ToothAdvised> GetToothAdvise(AdvisoryToothAdvise val)
        {
            var query = SearchQuery();

            if (val.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == val.CustomerId);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId);
            }

            var toothIds = await query.Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .SelectMany(x => x.AdvisoryToothRels).Select(x => x.ToothId).Distinct().ToListAsync();

            var res = new ToothAdvised();

            res.ToothIds = toothIds;

            return res;
        }

        public async Task<AdvisoryPrintVM> Print(Guid customerId, IEnumerable<Guid> ids)
        {
            var _partnerService = GetService<IPartnerService>();
            var res = new AdvisoryPrintVM();

            var partner = await _partnerService.SearchQuery(x => x.Id == customerId).Include(x => x.Company.Partner).FirstOrDefaultAsync();
            if (partner == null) return null;

            res.Partner = _mapper.Map<PartnerDisplay>(partner);
            res.Company = _mapper.Map<CompanyPrintVM>(partner.Company);

            var query = SearchQuery();

            if (ids != null && ids.Count() > 0)
            {
                query = query.Where(x => ids.Contains(x.Id));
            }

            var advisories = await query
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.User).ToListAsync();

            res.Advisories = _mapper.Map<IEnumerable<AdvisoryDisplay>>(advisories);

            return res;
        }

        public async Task<SaleOrderBasic> CreateSaleOrder(CreateFromAdvisoryInput val)
        {
            var _saleOrderService = GetService<ISaleOrderService>();
            var saleOrderDefaultGet = new SaleOrderDefaultGet();
            saleOrderDefaultGet.PartnerId = val.CustomerId;
            var saleOrderDisplay = await _saleOrderService.DefaultGet(saleOrderDefaultGet);
            //var saleOrder = _mapper.Map<SaleOrder>(saleOrderDisplay);
            var saleOrder = new SaleOrder();
            saleOrder.PartnerId = saleOrderDisplay.PartnerId;
            saleOrder.State = saleOrderDisplay.State;
            saleOrder.CompanyId = saleOrderDisplay.CompanyId;
            var result = await _saleOrderService.CreateAsync(saleOrder);

            var query = SearchQuery();

            if (val.Ids != null && val.Ids.Any())
            {
                query = query.Where(x => val.Ids.Contains(x.Id));
            }

            var toothDiagnosisName = await query.Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .SelectMany(x => x.AdvisoryToothDiagnosisRels).Select(x => x.ToothDiagnosis.Name).ToListAsync();

            var products = await query.Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .SelectMany(x => x.AdvisoryProductRels).Select(x => x.Product).ToListAsync();

            var toothIds = await query.Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .SelectMany(x => x.AdvisoryToothRels).Select(x => x.ToothId).ToListAsync();

            var saleOrderLines = new List<SaleOrderLine>();
            var sequence = 0;
            foreach (var product in products)
            {
                var saleOrderLine = new SaleOrderLine();
                saleOrderLine.ProductId = product.Id;
                saleOrderLine.Sequence = sequence++;
                saleOrderLine.AmountResidual = saleOrderLine.PriceSubTotal - saleOrderLine.AmountPaid;

                foreach (var toothId in toothIds)
                {
                    saleOrderLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                    {
                        ToothId = toothId
                    });
                }

                saleOrderLine.Diagnostic = string.Join(", ", toothDiagnosisName);

                saleOrderLines.Add(saleOrderLine);
            }

            return null;
        }

        //public async Task<QuotationSimple> CreateQuotation(CreateFromAdvisoryInput val)
        //{

        //}

        public async Task<PagedResult2<AdvisoryLine>> GetAdvisoryLines(AdvisoryLinePaged val)
        {
            var advisoryService = GetService<IAdvisoryService>();
            var saleOrderLineService = GetService<ISaleOrderLineService>();
            var quotationLineService = GetService<IQuotationLineService>();
            var res = new List<AdvisoryLine>();
            var saleOrderLines = await saleOrderLineService.SearchQuery(x => x.AdvisoryId == val.AdvisoryId).Include(x => x.Order).ToListAsync();
            if (saleOrderLines.Any())
            {
                foreach (var line in saleOrderLines)
                {
                    res.Add(new AdvisoryLine
                    {
                        Id = line.Id,
                        Name = line.Order.Name,
                        ProductName = line.Name,
                        Date = line.Order.DateOrder,
                        DoctorName = line.Employee != null ? line.Employee.Name : null,
                        Qty = line.ProductUOMQty,
                        Type = "saleOrder"
                    });
                }
            }

            var quotationLines = await quotationLineService.SearchQuery(x => x.AdvisoryId == val.AdvisoryId).Include(x => x.Quotation).ToListAsync();
            if (quotationLines.Any())
            {
                foreach (var line in quotationLines)
                {
                    res.Add(new AdvisoryLine
                    {
                        Id = line.Id,
                        Name = line.Quotation.Name,
                        ProductName = line.Name,
                        Date = line.Quotation.DateQuotation,
                        Qty = line.Qty,
                        Type = "quotation"
                    });
                }
            }

            var totalItems = res.Count();



            res = res.OrderByDescending(x => x.Date).ToList();

            var items = res.Skip(val.Offset).Take(val.Limit).ToList();

            var paged = new PagedResult2<AdvisoryLine>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }
    }
}
