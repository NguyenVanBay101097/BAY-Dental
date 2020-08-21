using ApplicationCore.Entities;
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
        Task<IEnumerable<CommissionSettlementReportItemOutput>> GetReportDetail(CommissionSettlementReportItem val);
    }
}
