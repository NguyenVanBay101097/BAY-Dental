using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IMarketingCampaignActivityService: IBaseService<MarketingCampaignActivity>
    {
        void CheckAutoTakeCoupon(IEnumerable<MarketingCampaignActivity> self);
        Task<MarketingCampaignActivityDisplay> GetActivityDisplay(Guid id);
        Task<MarketingCampaignActivity> CreateActivity(MarketingCampaignActivitySave val);
        Task UpdateActivity(Guid id, MarketingCampaignActivitySave val);
        Task RemoveActivity(IEnumerable<Guid> ids);

    }
}
