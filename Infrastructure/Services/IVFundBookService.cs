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
        Task<PagedResult2<VFundBookDisplay>> GetMoney(VFundBookSearch val);
        Task<CashBookReport> GetSumary(VFundBookSearch val);
        Task<List<FundBookExportExcel>> GetExportExcel(VFundBookSearch val);
    }
}
