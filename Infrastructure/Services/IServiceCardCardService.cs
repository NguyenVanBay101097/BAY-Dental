using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IServiceCardCardService: IBaseService<ServiceCardCard>
    {
        Task ActionActive(IEnumerable<ServiceCardCard> self);
        Task<PagedResult2<ServiceCardCardBasic>> GetPagedResultAsync(ServiceCardCardPaged val);
    }
}
