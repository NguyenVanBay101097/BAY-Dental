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
        Task Unlink(IEnumerable<DotKhamStep> self);
        Task<DotKhamStep> CloneInsert(DotKhamStepCloneInsert val);
        Task<DotKhamStepDisplay> GetDisplay(Guid id);
    }
}
