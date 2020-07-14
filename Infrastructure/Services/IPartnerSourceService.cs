using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerSourceService : IBaseService<PartnerSource>
    {
        Task<PagedResult2<PartnerSourceBasic>> GetPagedResultAsync(PartnerSourcePaged val);
        Task<IEnumerable<PartnerSourceBasic>> GetAutocompleteAsync(PartnerSourcePaged val);
        Task<List<ReportPartnerSourceItem>> GetReportPartnerSource(ReportFilterPartnerSource val);
    }
}
