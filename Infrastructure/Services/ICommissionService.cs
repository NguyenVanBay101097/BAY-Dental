using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICommissionService :IBaseService<Commission>
    {
        Task<PagedResult2<CommissionBasic>> GetPagedResultAsync(CommissionPaged val);
        Task<CommissionDisplay> GetCommissionForDisplay(Guid id);
        Task<Commission> CreateCommission(CommissionSave val);
        Task UpdateCommission(Guid id, CommissionDisplay val);

    }
}
