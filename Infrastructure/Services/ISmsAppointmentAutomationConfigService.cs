﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsAppointmentAutomationConfigService : IBaseService<SmsAppointmentAutomationConfig>
    {
        Task UpdateAsync(Guid id, SmsAppointmentAutomationConfigSave val);
        Task<SmsAppointmentAutomationConfigDisplay> GetByCompany(Guid compayId);
    }
}
