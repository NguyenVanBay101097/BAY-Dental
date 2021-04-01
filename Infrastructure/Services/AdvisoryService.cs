using ApplicationCore.Entities;
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

            query = query.Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
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
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();

            var res = _mapper.Map<AdvisoryDisplay>(advisory);

            return res;
        }

        public async Task<AdvisorySave> CreateAdvisory(AdvisorySave val)
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

            return val;
        }

        public async Task UpdateAdvisory(Guid id, AdvisorySave val)
        {
            var advisory = await SearchQuery(x => x.Id == id)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.AdvisoryToothDiagnosisRels).ThenInclude(x => x.ToothDiagnosis)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();
            advisory = _mapper.Map(val, advisory);
            // Xóa chuẩn đoán răng
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

        public async Task Unlink(Guid id)
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
    }
}
