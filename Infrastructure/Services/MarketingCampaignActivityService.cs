using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public class MarketingCampaignActivityService : BaseService<MarketingCampaignActivity>, IMarketingCampaignActivityService
    {
        public MarketingCampaignActivityService(IAsyncRepository<MarketingCampaignActivity> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public void CheckAutoTakeCoupon(IEnumerable<MarketingCampaignActivity> self)
        {
            //if (self.Any(x => x.AutoTakeCoupon == true && !x.CouponProgramId.HasValue))
            //    throw new Exception("Vui lòng chọn 1 chương trình coupon");
            //foreach(var activity in self)
            //{
            //    if (activity.AutoTakeCoupon != true)
            //        continue;
            //    if (!activity.CouponProgramId.HasValue)
            //        throw new Exception("Vui lòng chọn 1 chương trình coupon");
            //    var ma_tron = "{ma_coupon}";
            //    if (!activity.Content.Contains($"{ma_tron}"))
            //        throw new Exception($"Bạn nên chèn mã trộn {ma_tron} vào nội dung gửi");
            //}
        }
    }
}
