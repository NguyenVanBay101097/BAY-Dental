using ApplicationCore.Entities;
using ApplicationCore.Models;
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
        Task ToggleIsDone(DotKhamStepSetDone val);
        Task Unlink(IEnumerable<DotKhamStep> self);
        Task<DotKhamStep> CloneInsert(DotKhamStepCloneInsert val);
        Task<DotKhamStepDisplay> GetDisplay(Guid id);
        Task<PagedResult2<DotKhamStepReport>> DotKhamStepReport(DotKhamStepPaged val);
    }
}
