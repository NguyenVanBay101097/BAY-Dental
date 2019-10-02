using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResGroupService: IBaseService<ResGroup>
    {
        Task<PagedResult2<ResGroupBasic>> GetPagedResultAsync(ResGroupPaged val);
        Task<ResGroupDisplay> DefaultGet();
    }
}
