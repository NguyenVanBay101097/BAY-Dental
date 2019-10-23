using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountInvoiceLineService : BaseService<AccountInvoiceLine>, IAccountInvoiceLineService
    {
        private readonly IMapper _mapper;
        public AccountInvoiceLineService(IAsyncRepository<AccountInvoiceLine> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public void ComputePrice(IEnumerable<AccountInvoiceLine> self)
        {
            foreach (var line in self)
            {
                var price = line.PriceUnit * (1 - line.Discount / 100);
                line.PriceSubTotal = price * line.Quantity;
                var priceSubTotalSigned = line.PriceSubTotal;
                var sign = line.Invoice.Type == "in_refund" || line.Invoice.Type == "out_refund" ? -1 : 1;
                line.PriceSubTotalSigned = priceSubTotalSigned * sign;
            }
        }

        public async Task<AccountInvoiceLineDisplay> DefaultGet(AccountInvoiceLineDefaultGet val)
        {
            var account = await _DefaultAccount(val.JournalId, val.Type);
            var res = new AccountInvoiceLineDisplay();
            if (account == null)
                throw new Exception("Không tìm thấy account cho chi tiết hóa đơn.");
            res.AccountId = account.Id;
            return res;
        }

        private async Task<AccountAccount> _DefaultAccount(Guid journalId, string type)
        {
            var journalObj = GetService<IAccountJournalService>();
            var journal = await journalObj.GetJournalWithDebitCreditAccount(journalId);
            if (journal != null)
            {
                if (type == "out_invoice" || type == "in_refund")
                    return journal.DefaultCreditAccount;
                return journal.DefaultDebitAccount;
            }
            return null;
        }

        public void UpdateRelatedData(ICollection<AccountInvoiceLine> self, AccountInvoice invoice)
        {
            foreach (var line in self)
            {
                line.Invoice = invoice;
                line.CompanyId = invoice.CompanyId;
                line.PartnerId = invoice.PartnerId;
            }
        }

        public async Task<AccountInvoiceLineOnChangeProductResult> OnChangeProduct(AccountInvoiceLineOnChangeProduct val)
        {
            var res = new AccountInvoiceLineOnChangeProductResult();

            var companyObj = GetService<ICompanyService>();
            var companyId = CompanyId;
            var company = await companyObj.SearchQuery(x => x.Id == companyId).Include(x => x.AccountIncome)
                .Include(x => x.AccountExpense).FirstOrDefaultAsync();
            if (val.InvoiceType == "out_invoice" || val.InvoiceType == "out_refund")
                res.AccountId = company.AccountIncome.Id;
            else
                res.AccountId = company.AccountExpense.Id;

            if (!val.ProductId.HasValue)
            {
                res.PriceUnit = 0;
            }
            else
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId).Include(x => x.Categ).FirstOrDefaultAsync();
                res.Name = product.Name;
                var computePrice = await productObj._ComputeProductPrice(new List<Product>() { product },val.PartnerId);
                res.PriceUnit = computePrice[product.Id];
                res.UoMId = product.UOMId;
            }

            return res;
        }

        //Lấy các product trong hóa đơn theo id của đợt khám
        public async Task<IEnumerable<AccountInvoiceLineSimple>> GetDotKhamInvoiceLine(Guid id)
        {
            var dotKhamObj = GetService<IDotKhamService>();
            var invoice = await dotKhamObj.GetByIdAsync(id);
            var accInvLines = await SearchQuery(x => x.InvoiceId == invoice.InvoiceId).ToListAsync();

            var res = _mapper.Map<IEnumerable<AccountInvoiceLineSimple>>(accInvLines);

            return res;
        }
    }
}
