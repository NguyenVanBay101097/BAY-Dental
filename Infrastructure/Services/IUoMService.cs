using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IUoMService: IBaseService<UoM>
    {
        Task<UoM> DefaultUOM();
        Task<PagedResult2<UoMBasic>> GetPagedResultAsync(UoMPaged val);
        decimal ComputePrice(UoM fromUOM, decimal price, UoM toUOM);
        decimal ComputeQtyObj(UoM fromUOM, decimal qty, UoM toUOM, string roundingMethod = "UP");
    }
}
