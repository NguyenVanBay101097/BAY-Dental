using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPrintTemplateService : IBaseService<PrintTemplate>
    {
        Task<string> GetPrintTemplate(PrintTemplateDefault val);
        Task<string> RenderTemplate(PrintTemplate self, IEnumerable<Guid> resIds, IEnumerable<object> resObjs = null);
        Task<string> GeneratePrintHtml(PrintTemplate self, IEnumerable<Guid> resIds, PrintPaperSize paperSize = null, IEnumerable<object> resObjs = null);
        Task<PrintTemplate> GetDefaultTemplate(string type);
        string GetModelTemplate(string type);
    }
}
