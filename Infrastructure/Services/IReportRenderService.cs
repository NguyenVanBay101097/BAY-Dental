using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IReportRenderService
    {
        Task<string> RenderHtml(PrintTemplateConfig report, IEnumerable<Guid> docIds);

        Task<byte[]> RenderPdf(PrintTemplateConfig report, IEnumerable<Guid> docIds);
    }
}
