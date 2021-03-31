using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDotKhamLineService : IBaseService<DotKhamLine>
    {
        Task<PagedResult2<DotKhamLineBasic>> GetPagedResultAsync(DotKhamLinePaged val);
        Task<IEnumerable<IEnumerable<DotKhamLineBasic>>> GetAllForDotKham2(Guid dotKhamId);
        Task CheckUpdateStartOperation(Guid id);
    }
}
