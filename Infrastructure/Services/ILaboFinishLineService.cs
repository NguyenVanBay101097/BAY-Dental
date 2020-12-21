using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboFinishLineService: IBaseService<LaboFinishLine>
    {
        Task<PagedResult2<LaboFinishLineBasic>> GetPagedResultAsync(LaboFinishLinesPaged val);
        Task<LaboFinishLineDisplay> GetDisplay(Guid id);
        Task<LaboFinishLineDisplay> CreateItem(LaboFinishLineSave val);
        Task UpdateItem(Guid id,LaboFinishLineSave val);
    }
}
