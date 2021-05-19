﻿using ApplicationCore.Entities;
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
        Task<SmsCampaignBasic> CreateAsync(SmsCampaignSave val);
        Task UpdateAsync(Guid id, SmsCampaignSave val);
        Task<SmsCampaign> GetDefaultCampaignBirthday();
        Task<SmsCampaign> GetDefaultCampaignAppointmentReminder();
        Task<SmsCampaign> GetDefaultCampaign();
        Task<SmsCampaignBasic> GetDisplay(Guid id);
    }
}