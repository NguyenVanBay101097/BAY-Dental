using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareCampaignService: IBaseService<TCareCampaign>
    {
        Task<PagedResult2<TCareCampaignBasic>> GetPagedResultAsync(TCareCampaignPaged val);
        Task<TCareCampaign> NameCreate(TCareCampaignNameCreateVM val);
        Task SetSheduleStart(TCareCampaignSetSheduleStart val);
        Task ActionStartCampaign(TCareCampaignStart val);
        Task ActionStopCampaign(IEnumerable<Guid> ids);
    }
}
