using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ResInsurancePaymentService : BaseService<ResInsurancePayment>, IResInsurancePaymentService
    {
        private readonly IMapper _mapper;

        public ResInsurancePaymentService(IAsyncRepository<ResInsurancePayment> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ResInsurancePayment> CreateResInsurancePayment(ResInsurancePaymentSave val)
        {
            var resInsurancePayment = _mapper.Map<ResInsurancePayment>(val);
            SaveLines(val, resInsurancePayment);
            await CreateAsync(resInsurancePayment);

            resInsurancePayment = await SearchQuery(x => x.Id == resInsurancePayment.Id)
                .Include(x => x.Lines).ThenInclude(s => s.SaleOrderLine)
                .FirstOrDefaultAsync();

            ComputeAmount(resInsurancePayment);

            if (resInsurancePayment.Amount <= 0)
                throw new Exception("Tổng số tiền bảo hiểm trả phải lớn hơn 0");

            await UpdateAsync(resInsurancePayment);

            return resInsurancePayment;
        }

        private void SaveLines(ResInsurancePaymentSave val, ResInsurancePayment insurancePayment)
        {
            var lineToRemoves = new List<ResInsurancePaymentLine>();

            foreach (var existLine in insurancePayment.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            if (lineToRemoves.Any())
            {
                foreach (var line in lineToRemoves)
                {
                    insurancePayment.Lines.Remove(line);
                }
            }


            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<ResInsurancePaymentLine>(line);
                    insurancePayment.Lines.Add(item);
                }
                else
                {
                    var l = insurancePayment.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                }
            }
        }

        private void ComputeAmount(ResInsurancePayment self)
        {
            var totalAmount = 0M;

            foreach (var line in self.Lines)
            {
                var amount = line.PayType == "percent" ? line.SaleOrderLine.PriceTotal *  ((line.Percent ?? 0) / 100) : (line.FixedAmount ?? 0);
                totalAmount += amount;
            }

            self.Amount = totalAmount;
        }

        public async Task ActionPayment(IEnumerable<Guid> ids)
        {
            var journalObj = GetService<IAccountJournalService>();
            var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
            var insurancePayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.ResInsurance)
                .Include(x => x.Lines).ThenInclude(x => x.SaleOrderLine)
                .ToListAsync();

            var reference_dict = new Dictionary<Guid, Guid>();

            foreach(var insurancePayment in insurancePayments)
            {
                var saleOrderPayment = new SaleOrderPayment();
                saleOrderPayment.CompanyId = insurancePayment.CompanyId;
                saleOrderPayment.OrderId = insurancePayment.OrderId.Value;
                saleOrderPayment.Date = insurancePayment.Date;
                saleOrderPayment.Note = insurancePayment.Note;
                saleOrderPayment.Amount = insurancePayment.Amount;

                ///add SaleOrderPaymentHistoryLine
                foreach (var line in insurancePayment.Lines)
                {
                    var amount = line.PayType == "percent" ? (line.SaleOrderLine.AmountResidual ?? 0) * ((line.Percent ?? 0) / 100) : (line.FixedAmount ?? 0);
                    saleOrderPayment.Lines.Add(new SaleOrderPaymentHistoryLine
                    {
                        SaleOrderLineId = line.SaleOrderLineId,
                        Amount = amount,
                        
                    });
                }

                ///add journallines
                var journal = await journalObj.GetJournalByTypeAndCompany("insurance", saleOrderPayment.CompanyId);
                if (journal == null)
                    throw new Exception("Không tìm thấy phương thức bảo hiểm");

                saleOrderPayment.JournalLines.Add(new SaleOrderPaymentJournalLine
                {
                    JournalId = journal.Id,
                    Amount = saleOrderPayment.Lines.Sum(x => x.Amount),
                    InsuranceId = insurancePayment.ResInsurance.Id
                });

                await saleOrderPaymentObj.CreateAsync(saleOrderPayment);

                reference_dict.Add(insurancePayment.Id, saleOrderPayment.Id);           
            }


            ///thanh toan
            var paymentIds = reference_dict.Values.Select(x => x);
            await saleOrderPaymentObj.ActionPayment(paymentIds);

            foreach(var item in insurancePayments)
            {
                if (!reference_dict.ContainsKey(item.Id))
                    continue;

                item.SaleOrderPaymentId = reference_dict[item.Id];
                item.State = "posted";
            }


            await UpdateAsync(insurancePayments);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var insurancePamentLineObj = GetService<IResInsurancePaymentLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ToListAsync();
            
            foreach(var item in self)
            {
                if (item.Lines.Any())
                    await insurancePamentLineObj.DeleteAsync(item.Lines);

            }

            await DeleteAsync(self);
        }

        public override ISpecification<ResInsurancePayment> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.res_insurance_payment_comp_rule":
                    return new InitialSpecification<ResInsurancePayment>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
