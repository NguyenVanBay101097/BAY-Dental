using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ICommissionSettlementService : IBaseService<CommissionSettlement>
    {
        Task CreateSettlements(AccountPayment val);
        Task Unlink(IEnumerable<Guid> paymentIds);
    }
}
