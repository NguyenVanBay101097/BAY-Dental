using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPromotionProgramService: IBaseService<PromotionProgram>
    {
        Task<PagedResult2<PromotionProgramBasic>> GetPagedResultAsync(PromotionProgramPaged val);
        Task<PromotionProgram> CreateProgram(PromotionProgramSave val);
        Task UpdateProgram(Guid id, PromotionProgramSave val);
        Task<PromotionProgramDisplay> GetDisplay(Guid id);
        Task ToggleActive(IEnumerable<Guid> ids);
    }
}
