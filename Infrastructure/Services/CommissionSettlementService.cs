using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CommissionSettlementService : BaseService<CommissionSettlement>, ICommissionSettlementService
    {
        private readonly IMapper _mapper;
        public CommissionSettlementService(IAsyncRepository<CommissionSettlement> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task CreateSettlements(AccountPayment val)
        {
            var settlements = new List<CommissionSettlement>();
            var rels = val.SaleOrderLinePaymentRels;
            foreach (var rel in rels)
                settlements.Add(await _GetSettlement(rel));

            ComputeAmount(settlements);

            await CreateAsync(settlements);
        }

        public async Task<CommissionSettlement> _GetSettlement(SaleOrderLinePaymentRel val)
        {
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var linePaymentRel = await linePaymentRelObj.SearchQuery(x => x.Id == val.Id).Include(x => x.Payment).Include(x => x.SaleOrderLine).Include("SaleOrderLine.PartnerCommissions").FirstOrDefaultAsync();

            var employeeObj = GetService<IEmployeeService>();
            var employee = await employeeObj.SearchQuery(x => x.UserId == linePaymentRel.SaleOrderLine.SalesmanId)
                .Include(x => x.Commission).FirstOrDefaultAsync();
            if (employee == null || employee.Commission == null)
                return null;


            var userObj = GetService<IUserService>();
            var user = await userObj.GetByIdAsync(linePaymentRel.SaleOrderLine.SalesmanId);
            if (user == null)
                return null;

            var partnerCommissionObj = GetService<ISaleOrderLinePartnerCommissionService>();
            var partnerCommission = await partnerCommissionObj.SearchQuery(x => x.SaleOrderLineId == linePaymentRel.SaleOrderLineId && x.CommissionId == employee.CommissionId && x.PartnerId == user.PartnerId).FirstOrDefaultAsync();
            if (partnerCommission == null)
                return null;

            var res = new CommissionSettlement
            {
                Partner = user.Partner,
                PartnerId = user.PartnerId,
                Employee = employee,
                EmployeeId = employee.Id,
                SaleOrderLine = linePaymentRel.SaleOrderLine,
                SaleOrderLineId = linePaymentRel.SaleOrderLineId,
                Payment = linePaymentRel.Payment,
                PaymentId = linePaymentRel.PaymentId,
                BaseAmount = linePaymentRel.AmountPrepaid,
                Percentage = partnerCommission.Percentage
            };

            return res;

        }

        public IEnumerable<CommissionSettlement> ComputeAmount(IEnumerable<CommissionSettlement> val)
        {
            foreach (var item in val)
                item.Amount = (item.BaseAmount * item.Percentage) / 100;

            return val;

        }

        public async Task Unlink(IEnumerable<Guid> paymentIds)
        {
            var res = await SearchQuery(x => paymentIds.Contains(x.PaymentId.Value)).ToListAsync();
            if (res == null)
                throw new Exception("Null CommissionSettlement");

            await DeleteAsync(res);
        }

    }
}
