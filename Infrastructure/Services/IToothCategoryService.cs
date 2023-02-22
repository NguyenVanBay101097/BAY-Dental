using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToothCategoryService: IBaseService<ToothCategory>
    {
        Task<PagedResult2<ToothCategory>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "");
        Task<IEnumerable<ToothCategoryBasic>> GetAllBasic();
        Task<ToothCategoryBasic> GetDefaultCategory();
    }
}
