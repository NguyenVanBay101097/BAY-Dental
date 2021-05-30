using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToothService: IBaseService<Tooth>
    {
        Task<PagedResult2<Tooth>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "");
        Task<IEnumerable<ToothDisplay>> GetAllDisplay(ToothFilter val);
        Task<ToothDisplay> GetDisplay(Guid id);
    }
}
