using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IResConfigSettingsService: IBaseService<ResConfigSettings>
    {
        Task Excute<T>(T self);
        Task<T> DefaultGet<T>();
        Task InsertServiceCardData();
        Task InsertFieldForProductListPriceRestrictCompanies();
    }
}
