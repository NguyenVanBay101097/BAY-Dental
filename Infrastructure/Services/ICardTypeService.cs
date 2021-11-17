using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICardTypeService : IBaseService<CardType>
    {
        Task<PagedResult2<CardTypeBasic>> GetPagedResultAsync(CardTypePaged val);
        Task<CardType> CreateCardType(CardTypeDisplay val);
        Task UpdateCardType(Guid id, CardTypeDisplay val);
        DateTime GetPeriodEndDate(CardType self, DateTime? dStart = null);

        Task<IEnumerable<CardType>> GetAutoComplete(CardTypePaged val);
        Task AddProductPricelistItem(Guid id, IEnumerable<Guid> productIds);
        Task UpdateProductPricelistItem(Guid id, IEnumerable<ProductPricelistItemCreate> items);
        Task ApplyServiceCategories(Guid id, IEnumerable<ApplyServiceCategoryReq> vals);
        Task ApplyAllServices(Guid id, ApplyAllServiceReq val);
    }
}
