using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerTitleService : IBaseService<PartnerTitle>
    {
        Task<PagedResult2<PartnerTitleBasic>> GetPagedResultAsync(PartnerTitlePaged val);
        Task<IEnumerable<PartnerTitleBasic>> GetAutocompleteAsync(PartnerTitlePaged val);
    }
}
