using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerCategoryService : IBaseService<PartnerCategory>
    {
        Task<PagedResult2<PartnerCategoryBasic>> GetPagedResultAsync(PartnerCategoryPaged val);
        Task<IEnumerable<PartnerCategoryBasic>> GetAutocompleteAsync(PartnerCategoryPaged val);
        Task<PartnerCategory> CreatePartnerCategoryAsync(PartnerCategory categ);
        Task UpdatePartnerCategoryAsync(PartnerCategory categ);
    }
}
