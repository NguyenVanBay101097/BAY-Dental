using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboBiteJointService: IBaseService<LaboBiteJoint>
    {
        Task<PagedResult2<LaboBiteJointBasic>> GetPagedResultAsync(LaboBiteJointsPaged val);
        Task<LaboBiteJointDisplay> GetDisplay(Guid id);
        Task<LaboBiteJointDisplay> CreateItem(LaboBiteJointSave val);
        Task UpdateItem(Guid id,LaboBiteJointSave val);
        Task<IEnumerable<LaboBiteJointSimple>> Autocomplete(LaboBiteJointPageSimple val);

    }
}
