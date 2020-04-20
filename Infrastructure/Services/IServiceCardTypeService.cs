using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IServiceCardTypeService: IBaseService<ServiceCardType>
    {
        Task<PagedResult2<ServiceCardTypeBasic>> GetPagedResultAsync(ServiceCardTypePaged val);
        Task<ServiceCardType> CreateUI(ServiceCardTypeSave val);
        Task UpdateUI(Guid id, ServiceCardTypeSave val);
        DateTime GetPeriodEndDate(ServiceCardType self, DateTime? dStart = null);
    }
}
