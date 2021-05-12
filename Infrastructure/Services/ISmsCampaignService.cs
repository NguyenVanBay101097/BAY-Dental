using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsCampaignService : IBaseService<SmsCampaign>
    {
        Task<PagedResult2<SmsCampaignBasic>> GetPaged(SmsCampaignPaged val);
        Task<SmsCampaign> GetDefaultCampaignBirthday();
        Task<SmsCampaign> GetDefaultCampaignAppointmentReminder();
    }
}