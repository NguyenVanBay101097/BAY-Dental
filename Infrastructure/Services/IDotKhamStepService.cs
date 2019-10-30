﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDotKhamStepService : IBaseService<DotKhamStep>
    {
        Task<IEnumerable<DotKhamStep>> GetVisibleSteps(Guid dotKhamId, string show = "dotkham");
        Task<IEnumerable<IEnumerable<DotKhamStep>>> GetVisibleSteps2(Guid dotKhamId, string show = "dotkham");
        Task AssignDotKham(DotKhamStepAssignDotKhamVM val);
        Task ToggleIsDone(IEnumerable<Guid> ids);
    }
}
