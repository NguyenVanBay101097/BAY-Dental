using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductRequestService : IBaseService<ProductRequest>
    {
        Task<PagedResult2<ProductRequestBasic>> GetPagedResultAsync(ProductRequestPaged val);
        Task<ProductRequestDisplay> GetDisplay(Guid id);
        Task<ProductRequestDisplay> DefaultGet();

        Task<ProductRequest> CreateRequest(ProductRequestSave val);

        Task UpdateRequest(Guid id, ProductRequestSave val);

        Task ActionConfirm(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);

        Task ActionDone(IEnumerable<Guid> ids);

    }
}
