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
            var saleOrderObj = GetService<ISaleOrderService>();

            var saleOrder = await saleOrderObj.SearchQuery(x => x.Id == val.OrderId)
                .Include(x => x.OrderLines)
                .FirstOrDefaultAsync();

            #region saleorder payment
            var cashJournal = await journalObj.GetJournalByTypeAndCompany("cash", saleOrder.CompanyId);

            //if (!val.JournalLines.Any())
            //{
            //    val.JournalLines.Add(new SaleOrderPaymentJournalLineSave()
            //    {
            //        Amount = val.Amount,
            //        JournalId = cashJournal.Id
            //    });
            //}
            //if (val.Amount != val.Lines.Sum(x => x.Amount) || val.Amount != val.JournalLines.Sum(x => x.Amount))
            //    throw new Exception("Dữ liệu không đồng nhất!");

            //nếu ko thanh toán chi tiết thì tự động phân bổ
            var soPayment = new SaleOrderPayment
            {
                Amount = val.Amount,
                CompanyId = saleOrder.CompanyId,
                Date = val.Date,
                Note = val.Note,
                OrderId = saleOrder.Id,
            };

            if (!val.Lines.Any())
            {
                var amount = val.Amount;
                foreach(var line in saleOrder.OrderLines.OrderBy(x => x.Sequence))
                {
                    var amountResidual = line.PriceTotal - (line.AmountInvoiced ?? 0);
                    if (amount > amountResidual)
                    {
                        soPayment.Lines.Add(new SaleOrderPaymentHistoryLine
                        {
                            SaleOrderLineId = line.Id,
                            Amount = amountResidual
                        });

                        amount -= amountResidual;
                    }
                    else
                    {
                        soPayment.Lines.Add(new SaleOrderPaymentHistoryLine
                        {
                            SaleOrderLineId = line.Id,
                            Amount = amount
                        });

                        amount = 0;
                        break;
                    }
                }
            }
            else
            {
                foreach(var line in val.Lines)
                {
                    soPayment.Lines.Add(new SaleOrderPaymentHistoryLine
                    {
                        SaleOrderLineId = line.SaleOrderLineId,
                        Amount = line.Amount
                    });
                }
            }

            if (!val.JournalLines.Any())
            {
                soPayment.JournalLines.Add(new SaleOrderPaymentJournalLine
                {
                    JournalId = cashJournal.Id,
                    Amount = val.Amount
                });
            }
            else
            {
                foreach(var line in val.JournalLines)
                {
                    soPayment.JournalLines.Add(new SaleOrderPaymentJournalLine
                    {
                        JournalId = line.JournalId,
                        Amount = line.Amount
                    });
                }
            }

            await soPaymentObj.CreateAsync(soPayment);
            await soPaymentObj.ActionPayment(new List<Guid>() { soPayment.Id });

            #endregion
            #region debt payment
            if (val.IsDebtPayment && val.DebtJournalId.HasValue && val.DebtAmount > 0)
            {
                var phieuThuSave = new PhieuThuChiSave()
                {
                    AccountType = "customer_debt",
                    Amount = val.DebtAmount,
                    Date = val.Date,
                    JournalId = val.DebtJournalId.Value,
                    PartnerId = saleOrder.PartnerId,
                    Type = "thu",
                    Reason = val.DebtNote,
                };

                var phieuThu = await phieuThuChiObj.CreatePhieuThuChi(phieuThuSave);
                await phieuThuChiObj.ActionConfirm(new List<Guid>() { phieuThu.Id });
            }
         
            #endregion
            return soPayment;
        }

        private T GetService<T>()
        {
            return (T)_contextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

    }
}
