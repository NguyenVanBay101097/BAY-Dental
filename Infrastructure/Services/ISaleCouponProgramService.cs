using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleCouponProgramService: IBaseService<SaleCouponProgram>
    {
        Task Apply(SaleCouponProgram rule, SaleOrder order, decimal total_amount, decimal total_qty, SaleCoupon coupon = null);
        Task<PagedResult2<SaleCouponProgramBasic>> GetPagedResultAsync(SaleCouponProgramPaged val);
        Task<SaleCouponProgram> CreateProgram(SaleCouponProgramSave val);
        Task UpdateProgram(Guid id, SaleCouponProgramSave val);
        Task<SaleCouponProgramDisplay> GetDisplay(Guid id);
        Task GenerateCoupons(SaleCouponProgramGenerateCoupons val);
        Task Unlink(IEnumerable<Guid> ids);
        Task ToggleActive(IEnumerable<Guid> ids);
    }
}
