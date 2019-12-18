using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class SaleCouponService : BaseService<SaleCoupon>, ISaleCouponService
    {
        public SaleCouponService(IAsyncRepository<SaleCoupon> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override ISpecification<SaleCoupon> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale_coupon.sale_coupon_comp_rule":
                    return new InitialSpecification<SaleCoupon>(x => !x.Program.CompanyId.HasValue || x.Program.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<PagedResult2<SaleCouponBasic>> GetPagedResultAsync(SaleCouponPaged val)
        {
            ISpecification<SaleCoupon> spec = new InitialSpecification<SaleCoupon>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SaleCoupon>(x => x.Code.Contains(val.Search)));
            if (val.ProgramId.HasValue)
                spec = spec.And(new InitialSpecification<SaleCoupon>(x => x.ProgramId == val.ProgramId));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SaleCouponBasic
            {
                Id = x.Id,
                Code = x.Code,
                DateExpired = x.DateExpired,
                OrderName = x.Order.Name,
                PartnerName = x.Partner.Name,
                ProgramName = x.Program.Name,
                State = x.State
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleCouponBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SaleCouponDisplay> GetDisplay(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Select(x => new SaleCouponDisplay
            {
                Id = x.Id,
                Code = x.Code,
                DateExpired = x.DateExpired,
                OrderName = x.Order.Name,
                PartnerName = x.Partner.Name,
                ProgramName = x.Program.Name,
                State = x.State,
                OrderId = x.OrderId,
                PartnerId = x.PartnerId,
                ProgramId = x.ProgramId,
                SaleOrderId = x.SaleOrderId,
                SaleOrderName = x.SaleOrder.Name
            }).FirstOrDefaultAsync();
        }

        public override async Task<SaleCoupon> CreateAsync(SaleCoupon self)
        {
            await CheckCode(self);
            _ComputeDateExpired(self);
            return await base.CreateAsync(self);
        }

        public async Task UpdateDateExpired(IEnumerable<SaleCoupon> self)
        {
            _ComputeDateExpired(self);
            await UpdateAsync(self);
        }

        public async Task CheckCode(SaleCoupon self, int count = 1)
        {
            var exist = await SearchQuery(x => x.Code == self.Code).FirstOrDefaultAsync();
            if (exist == null)
                return;
            if (count > 3)
                throw new Exception("Phát sinh mã coupon không thành công, thử lại sau");
            self.Code = StringUtils.RandomStringDigit(20);
            await CheckCode(self, count + 1);
        }

        public void _ComputeDateExpired(SaleCoupon self)
        {
            _ComputeDateExpired(new List<SaleCoupon>() { self });
        }

        public void _ComputeDateExpired(IEnumerable<SaleCoupon> self)
        {
            foreach(var coupon in self)
            {
                var date = (coupon.DateCreated ?? DateTime.Today).Date;
                var validity_duration = coupon.Program.ValidityDuration ?? 0;
                if (validity_duration > 0)
                    coupon.DateExpired = date.AddDays(validity_duration);
                else
                    coupon.DateExpired = null;
            }
        }
    }
}
