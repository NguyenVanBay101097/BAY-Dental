using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IMarketingCampaignActivityService: IBaseService<MarketingCampaignActivity>
    {
        void CheckAutoTakeCoupon(IEnumerable<MarketingCampaignActivity> self);
    }
}
