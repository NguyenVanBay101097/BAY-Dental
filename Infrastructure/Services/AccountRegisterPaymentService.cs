using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class AccountRegisterPaymentService : BaseService<AccountRegisterPayment>, IAccountRegisterPaymentService
    {
        public AccountRegisterPaymentService(IAsyncRepository<AccountRegisterPayment> repository, IHttpContextAccessor httpContextAccessor)
            :base(repository, httpContextAccessor)
        {
        }

        public IDictionary<string, int> MAP_INVOICE_TYPE_PAYMENT_SIGN
        {
            get
            {
                var res = new Dictionary<string, int>();
                res.Add("out_invoice", 1);
                res.Add("in_refund", 1);
                res.Add("in_invoice", -1);
                res.Add("out_refund", -1);
                return res;
            }
        }
        public IDictionary<string, string> MAP_INVOICE_TYPE_PARTNER_TYPE
        {
            get
            {
                var res = new Dictionary<string, string>();
                res.Add("out_invoice", "customer");
                res.Add("out_refund", "customer");
                res.Add("in_invoice", "supplier");
                res.Add("in_refund", "supplier");
                return res;
            }
        }

        public async Task<AccountRegisterPaymentDisplay> DefaultGet(IEnumerable<Guid> invoice_ids)
        {
            if (invoice_ids == null || invoice_ids.Count() == 0)
                throw new Exception("Phải chọn tối thiểu 1 hóa đơn để thanh toán");
            var invoiceService = (IAccountInvoiceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountInvoiceService));
            var invoices = await invoiceService.SearchQuery(x => invoice_ids.Contains(x.Id)).ToListAsync();
            if (invoices.Any(x => x.State != "open" && x.State != "paid"))
                throw new Exception("Bạn chỉ có thể thanh toán cho hóa đơn đã xác nhận");
            if (invoices.Any(x => x.PartnerId != invoices[0].PartnerId))
                throw new Exception("Để thanh toán nhiều hóa đơn cùng một lần, chúng phải có cùng khách hàng/nhà cung cấp");
            var dict = MAP_INVOICE_TYPE_PAYMENT_SIGN;
            if (invoices.Any(x => dict[x.Type] != dict[invoices[0].Type]))
                throw new Exception("Bạn không thể kết hợp hóa đơn khách hàng và hóa đơn nhà cung cấp trong một lần thanh toán");
            //if (invoices.Any(x => x.CurrencyId != invoices[0].CurrencyId))
            //    throw new Exception("Các hóa đơn phải có chung tiền tệ");

            var total_amount = invoices.Sum(x => x.Residual * dict[x.Type]);
            var communication = string.Join(" ", invoices.Select(x => x.Number).Distinct());

            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount),
                PaymentType = total_amount > 0 ? "inbound" : "outbound",
                PartnerId = invoices[0].PartnerId,
                PartnerType = MAP_INVOICE_TYPE_PARTNER_TYPE[invoices[0].Type],
                Communication = communication,
                InvoiceIds = invoice_ids
            };

            return rec;
        }

        public async Task<AccountPayment> CreatePayment(Guid Id)
        {
            var self = await SearchQuery(x => x.Id == Id)
                .Include(x => x.Partner)
                .Include(x => x.Journal).Include(x => x.Journal.Sequence)
                .Include(x => x.Journal.Company)
                .Include(x => x.Journal.DefaultCreditAccount)
                .Include(x => x.Journal.DefaultDebitAccount)
                .Include(x => x.AccountRegisterPaymentInvoiceRels)
                .Include("AccountRegisterPaymentInvoiceRels.Invoice")
                .Include("AccountRegisterPaymentInvoiceRels.Invoice.Move")
                .Include("AccountRegisterPaymentInvoiceRels.Invoice.Account")
                .Include("AccountRegisterPaymentInvoiceRels.Invoice.Account.Company")
                .FirstOrDefaultAsync();
            var paymentObj = GetService<IAccountPaymentService>();
            var payment = await paymentObj.CreateAsync(GetPaymentVals(self));
            await paymentObj.Post(new List<AccountPayment>() { payment });
            return payment;
        }

        public AccountPayment GetPaymentVals(AccountRegisterPayment self)
        {
            var res = new AccountPayment
            {
                JournalId = self.JournalId,
                Journal = self.Journal,
                PaymentDate = self.PaymentDate,
                Communication = self.Communication,
                PaymentType = self.PaymentType,
                Amount = self.Amount,
                PartnerId = self.PartnerId,
                Partner = self.Partner,
                PartnerType = self.PartnerType,
                Company = self.Journal.Company,
                CompanyId = self.Journal.CompanyId,
            };

            foreach (var rel in self.AccountRegisterPaymentInvoiceRels)
            {
                res.AccountInvoicePaymentRels.Add(new AccountInvoicePaymentRel
                {
                    InvoiceId = rel.InvoiceId,
                    Invoice = rel.Invoice,
                    Payment= res,
                    PaymentId = res.Id
                });
            }
               
            return res;
        }
    }
}
