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
        Task<PagedResult2<UoM>> GetPagedResultAsync(UoMPaged val);

        UoMSave CheckRoundingAndCalculateFactor(UoMSave uom);
    }
}
