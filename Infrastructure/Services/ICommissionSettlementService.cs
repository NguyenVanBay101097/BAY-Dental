using ApplicationCore.Entities;
using ApplicationCore.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICommissionSettlementService : IBaseService<CommissionSettlement>
    {
        //Task CreateSettlements(AccountPayment val);
        Task Unlink(IEnumerable<Guid> paymentIds);

        Task<IEnumerable<CommissionSettlementReportOutput>> GetReport(CommissionSettlementFilterReport val);
        Task<PagedResult2<CommissionSettlementReportRes>> GetReportPaged(CommissionSettlementFilterReport val);
        Task<PagedResult2<CommissionSettlementReportDetailOutput>> GetReportDetail(CommissionSettlementFilterReport val);
        Task<decimal> GetSumReport(CommissionSettlementFilterReport val);

        string CommissionType(string commType);

    }
}
