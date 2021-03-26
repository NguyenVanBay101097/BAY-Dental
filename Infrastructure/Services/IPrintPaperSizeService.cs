using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPrintPaperSizeService : IBaseService<PrintPaperSize>
    {
        Task<PagedResult2<PrintPaperSizeBasic>> GetPagedResultAsync(PrintPaperSizePaged val);

        Task<PrintPaperSize> CreatePrintPaperSize(PrintPaperSizeSave val);

        Task UpdatePrintPaperSize(Guid id, PrintPaperSizeSave val);
    }
}
