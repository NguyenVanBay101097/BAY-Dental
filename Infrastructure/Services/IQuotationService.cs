using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IQuotationService : IBaseService<Quotation>
    {
        Task<PagedResult2<QuotationBasic>> GetPagedResultAsync(QuotationPaged val);
        Task<IEnumerable<QuotationDisplay>> GetDisplay(Guid id);
        Task<QuotationBasic> CreateAsync(QuotationSave val);
        Task UpdateAsync(Guid id, QuotationSave val);
    }
}
