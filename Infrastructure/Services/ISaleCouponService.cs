using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleCouponService: IBaseService<SaleCoupon>
    {
        Task UpdateDateExpired(IEnumerable<SaleCoupon> self);
        Task<PagedResult2<SaleCouponBasic>> GetPagedResultAsync(SaleCouponPaged val);
        Task<SaleCouponDisplay> GetDisplay(Guid id);
        Task<CheckPromoCodeMessage> _CheckCouponCode(SaleCoupon self, SaleOrder order);
    }
}
