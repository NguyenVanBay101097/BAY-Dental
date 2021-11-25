using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class PaymentService : IPaymentService
    {
        private readonly IMapper _mapper;
        IHttpContextAccessor _contextAccessor;
        public PaymentService(IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<SaleOrderPayment> SaleOrderCustomerDebtPayment(SaleOrderCustomerDebtPaymentReq val)
        {
            var soPaymentObj = GetService<ISaleOrderPaymentService>();
            var phieuThuChiObj = GetService<IPhieuThuChiService>();
            var journalObj = GetService<IAccountJournalService>();
            #region saleorder payment
            var cashJournal = await journalObj.GetJournalByTypeAndCompany("cash", val.CompanyId);

            if (!val.JournalLines.Any())
            {
                val.JournalLines.Add(new SaleOrderPaymentJournalLineSave()
                {
                    Amount = val.Amount,
                    JournalId = cashJournal.Id
                });
            }
            if (val.Amount != val.Lines.Sum(x => x.Amount) || val.Amount != val.JournalLines.Sum(x=> x.Amount))
                throw new Exception("Dữ liệu không đồng nhất!");
            var soPaymentSave = _mapper.Map<SaleOrderPaymentSave>(val);
            var soPayment = await soPaymentObj.CreateSaleOrderPayment(soPaymentSave);
            await soPaymentObj.ActionPayment(new List<Guid>() { soPayment.Id });
            #endregion
            #region debt payment
            if (!val.IsDebtPayment || val.DebtAmount == 0)
                return soPayment;
            var phieuThuChiSave = new PhieuThuChiSave()
            {
                AccountType = "customer_debt",
                Amount = val.DebtAmount,
                Date = val.Date,
                JournalId = val.DebtJournalId.HasValue ? val.DebtJournalId.Value : cashJournal.Id,
                PartnerId = val.PartnerId,
                Type = "thu",
                Reason = val.DebtNote
            };
            var phieuThuChi = await phieuThuChiObj.CreatePhieuThuChi(phieuThuChiSave);
            await phieuThuChiObj.ActionConfirm(new List<Guid>() { phieuThuChi.Id });
            #endregion
            return soPayment;
        }

        private T GetService<T>()
        {
            return (T)_contextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

    }
}
