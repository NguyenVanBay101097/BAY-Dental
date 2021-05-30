using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IConfigPrintService : IBaseService<ConfigPrint>
    {
        Task<IEnumerable<ConfigPrintBasic>> LoadConfigPrint();

        Task CreateUpdateConfigPrint(IEnumerable<ConfigPrintSave> vals);
    }
}
