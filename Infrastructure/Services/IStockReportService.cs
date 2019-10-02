using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IStockReportService
    {
        Task<IEnumerable<StockReportXuatNhapTonItem>> XuatNhapTonSummary(StockReportXuatNhapTonSearch val);
        Task<IEnumerable<StockReportXuatNhapTonItemDetail>> XuatNhapTonDetail(StockReportXuatNhapTonItem val);
    }
}
