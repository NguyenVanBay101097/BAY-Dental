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
        Task ActionActive(IEnumerable<Guid> ids);
        Task<PagedResult2<ServiceCardCardBasic>> GetPagedResultAsync(ServiceCardCardPaged val);
        void _ComputeResidual(IEnumerable<ServiceCardCard> self);
        Task<IEnumerable<ServiceCardCard>> _ComputeResidual(IEnumerable<Guid> ids);
        Task ButtonConfirm(IEnumerable<ServiceCardCard> self);
        Task Unlink(IEnumerable<Guid> ids);
    }
}
