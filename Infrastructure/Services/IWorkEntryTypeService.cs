using ApplicationCore.Entities;
using ApplicationCore.Models;
using Facebook.ApiClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IWorkEntryTypeService : IBaseService<WorkEntryType>
    {
        Task<PagedResult2<WorkEntryTypeDisplay>> GetPaged(WorkEntryTypePaged val);
    }
}
