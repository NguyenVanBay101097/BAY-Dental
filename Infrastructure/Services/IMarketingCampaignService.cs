using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IMarketingCampaignService: IBaseService<MarketingCampaign>
    {
        Task<PagedResult2<MarketingCampaignBasic>> GetPagedResultAsync(MarketingCampaignPaged val);
        Task ActionStartCampaign(IEnumerable<Guid> ids);
        Task ActionStopCampaign(IEnumerable<Guid> ids);
    }
}
