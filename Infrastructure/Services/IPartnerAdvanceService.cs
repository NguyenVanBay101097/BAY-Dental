using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerAdvanceService : IBaseService<PartnerAdvance>
    {
        Task<PagedResult2<PartnerAdvanceBasic>> GetPagedResultAsync(PartnerAdvancePaged val);
        Task<PartnerAdvanceDisplay> GetDisplayById(Guid id);

        Task<PartnerAdvancDefaultViewModel> DefaultGet(PartnerAdvanceDefaultFilter val);
        Task<decimal> GetSummary(PartnerAdvanceSummaryFilter val);
        Task<PartnerAdvance> CreatePartnerAdvance(PartnerAdvanceSave val);
        Task UpdatePartnerAdvance(Guid id, PartnerAdvanceSave val);

        Task ActionConfirm(IEnumerable<Guid> ids);

        Task<PartnerAdvancePrint> GetPartnerAdvancePrint(Guid id);
    }
}
