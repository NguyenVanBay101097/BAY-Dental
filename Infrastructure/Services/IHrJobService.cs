using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrJobService : IBaseService<HrJob>
    {
        Task<PagedResult2<HrJobBasic>> GetPagedResultAsync(HrJobPaged val);
        Task<PagedResult2<HrJobBasic>> AutoComplete(HrJobPaged val);

    }
}
