using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICommissionSettlementService : IBaseService<CommissionSettlement>
    {
        Task CreateSettlements(AccountPayment val);
        Task Unlink(IEnumerable<Guid> paymentIds);
        Task<IEnumerable<CommissionSettlementReportOutput>> GetReport(CommissionSettlementReport val);
        Task<PagedResult2<CommissionSettlementReportDetailOutput>> GetReportDetail(CommissionSettlementReport val);
    }
}
