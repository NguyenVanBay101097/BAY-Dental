using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IVFundBookService
    {
        Task<PagedResult2<AccountMoveLineCashBookVM>> GetMoney(CashBookSearch val);
        Task<CashBookReport> GetSumary(CashBookSearch val);
        Task<CashBookReport> GetTotalReport(CashBookSearch val);
        Task<List<FundBookExportExcel>> GetExportExcel(CashBookSearch val);
    }
}
