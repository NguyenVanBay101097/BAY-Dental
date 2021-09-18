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
        Task<string> RenderTemplate(PrintTemplate self, Guid id);
        Task<string> GeneratePrintHtml(PrintTemplate self, Guid id, PrintPaperSize paperSize = null);
        Task<PrintTemplate> GetDefaultTemplate(string type);
        string GetModelTemplate(string type);
    }
}
