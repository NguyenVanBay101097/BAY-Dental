using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICardCardService: IBaseService<CardCard>
    {
        Task<decimal?> ConvertAmountToPoint(decimal? amount);
        Task<PagedResult2<CardCardBasic>> GetPagedResultAsync(CardCardPaged val);
        Task ButtonConfirm(IEnumerable<Guid> ids);
        Task ButtonActive(IEnumerable<Guid> ids, bool check_basic_points = true);
        Task ButtonRenew(IEnumerable<Guid> ids, bool check_basic_points = true);
        Task ButtonCancel(IEnumerable<Guid> ids);
        Task ButtonLock(IEnumerable<Guid> ids);
        Task ButtonUnlock(IEnumerable<Guid> ids);
        Task ButtonReset(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);
        bool IsExpired(CardCard self);
        Task ButtonUpgradeCard(IEnumerable<CardCard> self);
        Task ButtonUpgradeCard(IEnumerable<Guid> ids);
        Task<CardCard> GetValidCard(Guid partnerId);
        Task _CheckUpgrade(IEnumerable<CardCard> self, decimal? points = null);
    }
}
