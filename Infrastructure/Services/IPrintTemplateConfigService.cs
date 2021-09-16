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
        Task<PrintTemplateConfig> GetPrintTemplateConfig(string type);

        Task<PrintTemplateConfig> ChangePaperSize(string type, Guid paperSizeId);

        Task<object> GetSampleData(string type);

        Task<string> PrintTest(string content, string type, Guid paperSizeId);

        Task<string> Print(object data, string type);

        Task<string> RenderTemplate(object data, string content);

    }
}
