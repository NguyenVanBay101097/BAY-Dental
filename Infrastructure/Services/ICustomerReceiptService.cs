using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICustomerReceiptService : IBaseService<CustomerReceipt>
    {
        Task<PagedResult2<CustomerReceiptBasic>> GetPagedResultAsync(CustomerReceiptPaged val);

        Task<CustomerReceipt> GetDisplayById(Guid id);

        Task<CustomerReceipt> CreateCustomerReceipt(CustomerReceiptSave val);

        Task UpdateCustomerReceipt(Guid id, CustomerReceiptSave val);

        Task<long> GetCountToday(CustomerReceiptGetCountVM val);
    }
}
