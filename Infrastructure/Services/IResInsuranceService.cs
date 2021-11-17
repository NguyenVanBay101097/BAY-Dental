﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResInsuranceService : IBaseService<ResInsurance>
    {
        Task<PagedResult2<ResInsuranceBasic>> GetAgentPagedResult(ResInsurancePaged val);

        Task<ResInsurance> GetDisplayById(Guid id);
    }
}
