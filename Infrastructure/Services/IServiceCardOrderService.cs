using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IServiceCardOrderService: IBaseService<ServiceCardOrder>
    {
        Task<PagedResult2<ServiceCardOrderBasic>> GetPagedResultAsync(ServiceCardOrderPaged val);
        Task<ServiceCardOrder> CreateUI(ServiceCardOrderSave val);
        Task UpdateUI(Guid id, ServiceCardOrderSave val);
    }
}
