using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPrintTemplateConfigService : IBaseService<PrintTemplateConfig>
    {
        Task<PrintTemplateConfigDisplay> GetDisplay(PrintTemplateConfigChangeType val);

        Task CreateOrUpdate(PrintTemplateConfigSave val);

        Task<object> GetSampleData(string type);

        string GetLayout(string content);

    }
}
