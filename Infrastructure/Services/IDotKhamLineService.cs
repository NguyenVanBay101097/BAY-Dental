using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDotKhamLineService: IBaseService<DotKhamLine>
    {
        Task MarkProgress(IEnumerable<Guid> ids);
        Task MarkDone(IEnumerable<Guid> ids);
        Task CheckDone(IEnumerable<Guid> ids);
        Task<IEnumerable<DotKhamLineBasic>> GetAllForDotKham(Guid dotKhamId);
        Task<IEnumerable<IEnumerable<DotKhamLineBasic>>> GetAllForDotKham2(Guid dotKhamId);
        Task CheckUpdateStartOperation(Guid id);
        Task ChangeRouting(DotKhamLineChangeRouting val);        
    }
}
