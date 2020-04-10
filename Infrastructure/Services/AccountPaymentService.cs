using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class AccountPaymentService : BaseService<AccountPayment>, IAccountPaymentService
    {
        private readonly IMapper _mapper;
        public AccountPaymentService(IAsyncRepository<AccountPayment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task Post(IEnumerable<AccountPayment> payments)
        {
            var seqObj = GetService<IIRSequenceService>();
            foreach (var rec in payments)
            {
                if (rec.State != "draft")
                    throw new Exception("Chỉ những thanh toán nháp mới được vào sổ.");
                if (rec.AccountInvoicePaymentRels.Any(x => x.Invoice.State != "open" && x.Invoice.State != "paid"))
                    throw new Exception("Chỉ được thanh toán cho những hóa đơn trạng thái đã xác nhận.");
                string sequenceCode = "";
                if (string.IsNullOrEmpty(rec.Name))
                {
                    if (rec.PaymentType == "transfer")
                    {
                        sequenceCode = "account.payment.transfer";
                    }
                    else
                    {
                        if (rec.PartnerType == "customer")
                        {
                            if (rec.PaymentType == "outbound")
                                sequenceCode = "account.payment.customer.refund";
                            if (rec.PaymentType == "inbound")
                                sequenceCode = "account.payment.customer.invoice";
                        }
                        if (rec.PartnerType == "supplier")
                        {
                            if (rec.PaymentType == "outbound")
                                sequenceCode = "account.payment.supplier.invoice";
                            if (rec.PaymentType == "inbound")
                                sequenceCode = "account.payment.supplier.refund";
                        }
                    }

                    rec.Name = await seqObj.NextByCode(sequenceCode);
                }

           

                var amount = rec.Amount * (rec.PaymentType == "outbound" || rec.PaymentType == "transfer" ? 1 : -1);
                var move = await _CreatePaymentEntry(rec, amount);
                rec.State = "posted";
                await UpdateAsync(rec);
            }
        }

        private async Task<AccountMove> _GetMoveVals(AccountPayment rec, AccountJournal journal = null)
        {
            var seqObj = GetService<IIRSequenceService>();
            journal = journal ?? rec.Journal;
            if (journal.Sequence == null)
                throw new Exception("The journal " + journal.Name + " does not have a sequence, please specify one.");
            var name = await seqObj.NextById(journal.SequenceId);
            return new AccountMove
            {
                Name = name,
                Date = rec.PaymentDate,
                Ref = rec.Communication ?? "",
                CompanyId = rec.CompanyId,
                Company = rec.Company,
                JournalId = journal.Id,
                Journal = journal,
            };
        }

        private async Task<AccountMove> _CreatePaymentEntry(AccountPayment rec, decimal amount)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var invoiceObj = GetService<IAccountInvoiceService>();
            var moveObj = GetService<IAccountMoveService>();

            var computeAmlRes = amlObj.ComputeAmountFields(amount);
            var debit = computeAmlRes.Debit;
            var credit = computeAmlRes.Credit;

            var move = await _GetMoveVals(rec);
            await moveObj.CreateAsync(move);

            var counterpartAml = _GetSharedMoveLineVals(rec, debit, credit, move, null);
            var vals = await _GetCounterpartMoveLineVals(rec, rec.AccountInvoicePaymentRels.Select(x => x.Invoice));
            counterpartAml.Name = vals.Name;
            counterpartAml.Journal = vals.Journal;
            counterpartAml.JournalId = vals.JournalId;
            counterpartAml.Account = vals.Account;
            counterpartAml.AccountId = vals.AccountId;
            counterpartAml.PaymentId = vals.PaymentId;
            counterpartAml.Payment = vals.Payment;
            counterpartAml.CompanyId = vals.Account.CompanyId;
            counterpartAml.Company = vals.Account.Company;

            await amlObj.CreateAsync(counterpartAml);

            var payment_diff = _ComputePaymentDifference(new List<AccountPayment>() { rec})[rec.Id];
            if (rec.PaymentDifferenceHandling == "reconcile" && payment_diff != 0)
            {
                var writeoffLine = _GetSharedMoveLineVals(rec, 0, 0, move, null);
                var writeoffComputeRes = amlObj.ComputeAmountFields(payment_diff);
                var totalResidualCompanySigned = rec.AccountInvoicePaymentRels.Sum(x => x.Invoice.ResidualSigned);
                var totalPaymentCompanySigned = rec.Amount;

                decimal amountWO = 0;
                if (rec.AccountInvoicePaymentRels.Any())
                {
                    var firstInvoice = rec.AccountInvoicePaymentRels.First().Invoice;
                    if (firstInvoice.Type == "in_invoice" || firstInvoice.Type == "out_refund")
                    {
                        amountWO = totalPaymentCompanySigned - totalResidualCompanySigned;
                    }
                    else
                    {
                        amountWO = totalResidualCompanySigned - totalPaymentCompanySigned;
                    }
                }
           

                var debitWO = amountWO > 0 ? amountWO : 0;
                var creditWO = amountWO < 0 ? -amountWO : 0;
                writeoffLine.Name = "Write-Off";
                writeoffLine.Account = rec.WriteoffAccount;
                writeoffLine.AccountId = rec.WriteoffAccount.Id;
                writeoffLine.Debit = debitWO;
                writeoffLine.Credit = creditWO;
                writeoffLine.CompanyId = rec.WriteoffAccount.CompanyId;
                writeoffLine.Company = rec.WriteoffAccount.Company;
                writeoffLine.Journal = rec.Journal;
                writeoffLine.JournalId = rec.JournalId;
                writeoffLine.Payment = rec;
                writeoffLine.PaymentId = rec.Id;
                await amlObj.CreateAsync(writeoffLine);

                if (counterpartAml.Debit != 0)
                    counterpartAml.Debit += creditWO - debitWO;
                if (counterpartAml.Credit != 0)
                    counterpartAml.Credit += debitWO - creditWO;
                await amlObj.CreateAsync(counterpartAml);
            }

            //Write counterpart lines
            var liquidityAmlDict = _GetSharedMoveLineVals(rec, credit, debit, move, null);
            var vals2 = _GetLiquidityMoveLineVals(rec, -amount);
            liquidityAmlDict.Name = vals2.Name;
            liquidityAmlDict.Journal = vals2.Journal;
            liquidityAmlDict.JournalId = vals2.JournalId;
            liquidityAmlDict.Account = vals2.Account;
            liquidityAmlDict.AccountId = vals2.AccountId;
            liquidityAmlDict.PaymentId = vals2.PaymentId;
            liquidityAmlDict.Payment = vals2.Payment;
            liquidityAmlDict.CompanyId = vals2.Account.CompanyId;
            liquidityAmlDict.Company = vals2.Account.Company;
            await amlObj.CreateAsync(liquidityAmlDict);

            if (rec.AccountInvoicePaymentRels.Any())
            {
                var invoiceIds = rec.AccountInvoicePaymentRels.Select(x => x.InvoiceId);
                await invoiceObj.RegisterPayment(invoiceIds, counterpartAml);
            }
            else if (rec.Partner != null)
            {
                //Tự động đối soát với tất cả account move line nợ từ cũ nhất tới mới nhất
                await AutoReconcilePayment(rec, counterpartAml);
            }

            await moveObj.Write(new List<AccountMove>() { move });
            await moveObj.ActionPost(new List<AccountMove>() { move });
            return move;
        }

        private async Task AutoReconcilePayment(AccountPayment self, AccountMoveLine paymentLine)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();

            var lineToReconcile = new List<AccountMoveLine>();
            //dựa vào partner type để tìm các account move line nợ
            if (self.Partner == null)
                return;
            var commercialPartnerId = self.Partner.Id;

            var debt_lines = moveLineObj.SearchQuery(x => x.Reconciled == false && x.Id != paymentLine.Id && x.PartnerId == commercialPartnerId &&
            ((self.PartnerType == "customer" && x.Account.InternalType == "receivable") 
            || (self.PartnerType == "supplier" && x.Account.InternalType == "payable")),
            orderBy: x => x.OrderBy(s => s.Date)).Include(x => x.Invoice).Include("Invoice.PaymentMoveLines").Include(x => x.Move).ToList();

            lineToReconcile.AddRange(debt_lines);

            var reconcileLines = lineToReconcile.Concat(new List<AccountMoveLine>() { paymentLine });
            await moveLineObj.Reconcile(reconcileLines.ToList());

            //trigger update
            moveLineObj._AmountResidual(reconcileLines);
            await moveLineObj.UpdateAsync(reconcileLines);

            var invoices_to_update = reconcileLines.Where(x => x.Invoice != null).Select(x => x.Invoice).Distinct().ToList();
            if (invoices_to_update.Any())
            {
                var invObj = GetService<IAccountInvoiceService>();
                invObj._ComputeResidual(invoices_to_update);
                invObj._ComputePayments(invoices_to_update);
                await invObj.UpdateAsync(invoices_to_update);

                //update sale order residual
                var invoiceIds = invoices_to_update.Select(x => x.Id).ToList();
                var saleLineObj = GetService<ISaleOrderLineService>();
                var saleOrderIds = await saleLineObj.SearchQuery(x => x.SaleOrderLineInvoiceRels.Any(s => invoiceIds.Contains(s.InvoiceLine.Invoice.Id)))
                    .Select(x => x.OrderId).Distinct().ToListAsync();
                if (saleOrderIds.Any())
                {
                    var saleObj = GetService<ISaleOrderService>();
                    await saleObj.RecomputeResidual(saleOrderIds);
                }
            }
        }

        private AccountMoveLine _GetLiquidityMoveLineVals(AccountPayment rec, decimal amount)
        {
            var name = rec.Name;
            if (rec.PaymentType == "transfer")
            {
                name = string.Format("Transfer to {0}", rec.Journal.Name);
            }

            var account = rec.PaymentType == "outbound" || rec.PaymentType == "transfer" ? rec.Journal.DefaultDebitAccount : rec.Journal.DefaultCreditAccount;
            var vals = new AccountMoveLine
            {
                Name = name,
                AccountId = account.Id,
                Account = account,
                PaymentId = rec.Id,
                Payment = rec,
                JournalId = rec.JournalId,
                Journal = rec.Journal,
            };

            return vals;
        }

        public IDictionary<Guid, decimal> _ComputePaymentDifference(IEnumerable<AccountPayment> self)
        {
            var res = self.ToDictionary(x => x.Id, x => 0M);
            foreach (var payment in self)
            {
                res[payment.Id] = _ComputePaymentDifference(payment.AccountInvoicePaymentRels.Select(x => x.Invoice),
                    payment.Amount);
            }

            return res;
        }

        public decimal _ComputePaymentDifference(IEnumerable<AccountInvoice> invoices, decimal amount)
        {
            if (!invoices.Any())
                return 0;
            var invoice = invoices.First();
            if (invoice.Type == "in_invoice" || invoice.Type == "out_refund")
            {
                return amount - _ComputeTotalInvoiceAmount(invoices);
            }
            else
            {
                return _ComputeTotalInvoiceAmount(invoices) - amount;
            }
        }

        public async Task<AccountRegisterPaymentDisplay> OrderDefaultGet(IEnumerable<Guid> saleOrderIds)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orders = await orderObj.SearchQuery(x => saleOrderIds.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineInvoice2Rels")
                .Include("OrderLines.SaleOrderLineInvoice2Rels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoice2Rels.InvoiceLine.Move")
                .ToListAsync();

            var invoice_ids = orders.SelectMany(x => x.OrderLines).SelectMany(x => x.SaleOrderLineInvoice2Rels)
                .Select(x => x.InvoiceLine).Select(x => x.Move.Id).Distinct();

            var moveObj = GetService<IAccountMoveService>();
            var invoices = await moveObj.SearchQuery(x => invoice_ids.Contains(x.Id)).ToListAsync();
            invoices = invoices.Where(x => moveObj.IsInvoice(x, include_receipts: true)).ToList();

            if (!invoices.Any() || invoices.Any(x => x.State != "posted"))
                throw new Exception("You can only register payments for open invoices");
            var dtype = invoices[0].Type;
            foreach (var inv in invoices.Skip(1))
            {
                if (inv.Type != dtype)
                {
                    if ((dtype == "in_refund" && inv.Type == "in_invoice") || (dtype == "in_invoice" || inv.Type == "in_refund"))
                        throw new Exception("You cannot register payments for vendor bills and supplier refunds at the same time.");
                    if ((dtype == "out_refund" && inv.Type == "out_invoice") || (dtype == "out_invoice" || inv.Type == "out_refund"))
                        throw new Exception("You cannot register payments for customer invoices and credit notes at the same time.");
                }
            }

            var total_amount = invoices.Sum(x => x.AmountResidual);

            var communication = !string.IsNullOrEmpty(invoices[0].InvoicePaymentRef) ? invoices[0].InvoicePaymentRef :
                (!string.IsNullOrEmpty(invoices[0].Ref) ? invoices[0].Ref : invoices[0].Name);
            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount ?? 0),
                PaymentType = total_amount > 0 ? "inbound" : "outbound",
                PartnerId = invoices[0].PartnerId,
                PartnerType = MAP_INVOICE_TYPE_PARTNER_TYPE[invoices[0].Type],
                InvoiceIds = invoice_ids,
                Communication = communication
            };

            return rec;
        }

        public IDictionary<string, string> MAP_INVOICE_TYPE_PARTNER_TYPE
        {
            get
            {
                var res = new Dictionary<string, string>();
                res.Add("out_invoice", "customer");
                res.Add("out_refund", "customer");
                res.Add("out_receipt", "customer");
                res.Add("in_invoice", "supplier");
                res.Add("in_refund", "supplier");
                res.Add("in_receipt", "supplier");
                return res;
            }
        }

        /// <summary>
        /// Thanh toán nợ, bắt đầu từ hóa đơn hết hạn sớm nhất của list phiếu điều trị truyền vào
        /// </summary>
        public void _ComputeResidualPayment(IEnumerable<SaleOrder> saleOrders, decimal amount)
        {
            foreach (var order in saleOrders)
            {
                var invoices = order.OrderLines.SelectMany(x=>x.SaleOrderLineInvoiceRels).Select(x=>x.InvoiceLine).Select(x=>x.Invoice)
                    .OrderByDescending(x=>x.DateDue).ToList();
                foreach (var inv in invoices)
                {
                    if (amount > inv.Residual)
                    {
                        amount -= inv.Residual;
                        inv.Reconciled = true;
                        inv.Residual = 0M;
                    }
                    else
                    {
                        inv.Residual -= amount;
                    }

                    if (amount == 0)
                        break;
                }
                if (amount == 0)
                    break;
            }
        }

        private decimal _ComputeTotalInvoiceAmount(IEnumerable<AccountInvoice> invoices)
        {
            var total = invoices.Sum(x => x.ResidualSigned);
            return Math.Abs(total);
        }


        private async Task<AccountMoveLine> _GetCounterpartMoveLineVals(AccountPayment rec, IEnumerable<AccountInvoice> invoices = null)
        {
            string name = "";
            if (rec.PaymentType == "transfer")
                name = rec.Name;
            else
            {
                name = "";
                if (rec.PartnerType == "customer")
                {
                    if (rec.PaymentType == "inbound")
                        name += "Khách hàng thanh toán";
                    else if (rec.PaymentType == "outbound")
                        name += "Hoàn tiền khách hàng";
                }
                else if (rec.PartnerType == "supplier")
                {
                    if (rec.PaymentType == "inbound")
                        name += "Nhà cung cấp hoàn tiền";
                    else if (rec.PaymentType == "outbound")
                        name += "Thanh toán nhà cung cấp ";
                }

                if (invoices != null)
                {
                    name += ": ";
                    foreach (var inv in invoices)
                    {
                        if (inv.Move != null)
                        {
                            name += inv.Number + ", ";
                        }
                    }
                    name = name.Substring(0, name.Length - 2);
                }
            }

            var account = await _ComputeDestinationAccount(rec);
            return new AccountMoveLine
            {
                Name = name,
                Account = account,
                AccountId = account.Id,
                Company = account.Company,
                CompanyId = account.CompanyId,
                Journal = rec.Journal,
                JournalId = rec.JournalId,
                Payment = rec,
                PaymentId = rec.Id
            };
        }

        public async Task<AccountAccount> _ComputeDestinationAccount(AccountPayment rec)
        {
            if (rec.AccountInvoicePaymentRels.Any())
            {
                return rec.AccountInvoicePaymentRels.First().Invoice.Account;
            }
            else if (rec.Partner != null)
            {
                var accountObj = GetService<IAccountAccountService>();
                if (rec.PartnerType == "customer")
                    return await accountObj.GetAccountReceivableCurrentCompany();
                else
                    return await accountObj.GetAccountPayableCurrentCompany();
            }

            throw new Exception("Không tìm thấy tài khoản kế toán");
        }

        private AccountMoveLine _GetSharedMoveLineVals(AccountPayment rec, decimal debit, decimal credit, AccountMove move, Guid? invoiceId = null)
        {
            var partnerId = rec.PaymentType == "inbound" || rec.PaymentType == "outbound" ? rec.Partner.Id : (Guid?)null;
            return new AccountMoveLine
            {
                PartnerId = partnerId,
                InvoiceId = invoiceId,
                Move = move,
                MoveId = move.Id,
                Debit = debit,
                Credit = credit,
                Ref = move.Ref,
                Date = move.Date,
                Journal = move.Journal,
                JournalId = move.JournalId,
            };
        }

        public async Task<PagedResult2<AccountPaymentBasic>> GetPagedResultAsync(AccountPaymentPaged val)
        {
            ISpecification<AccountPayment> spec = new InitialSpecification<AccountPayment>(x => true);
            if (!string.IsNullOrWhiteSpace(val.Search))
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) || x.Partner.Phone.Contains(val.Search)));

            if (!string.IsNullOrEmpty(val.PartnerType))
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PartnerType == val.PartnerType));

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                spec = spec.And(new InitialSpecification<AccountPayment>(x => states.Contains(x.State)));
            }

            if (val.PaymentDateFrom.HasValue)
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PaymentDate <= val.PaymentDateFrom));

            if (val.PaymentDateTo.HasValue)
            {
                var paymentDateTo = val.PaymentDateTo.Value.AddDays(1);
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PaymentDate < paymentDateTo));
            }

            if (val.SaleOrderId.HasValue)
            {
                var orderObj = GetService<ISaleOrderService>();
                var order = orderObj.SearchQuery(x => x.Id == val.SaleOrderId)
                    .Include(x => x.OrderLines)
                    .Include("OrderLines.SaleOrderLineInvoiceRels")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice.AccountInvoicePaymentRels")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice.AccountInvoicePaymentRels.Payment")
                    .FirstOrDefault();
                var apIds = order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Select(x => x.InvoiceLine).Select(x => x.Invoice)
                    .SelectMany(x => x.AccountInvoicePaymentRels).Select(x => x.Payment);

                spec = spec.And(new InitialSpecification<AccountPayment>(x => apIds.Any(y => y.Id == x.Id)));
            }

            if (val.PartnerId.HasValue)
            {
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PartnerId.Equals(val.PartnerId)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated), limit: val.Limit, offSet: val.Offset);

            var items = await _mapper.ProjectTo<AccountPaymentBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<AccountPaymentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task CancelAsync(IEnumerable<Guid> ids)
        {
            await CancelAsync(SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines)
                .Include("MoveLines.Move").Include("MoveLines.Move.Lines").Include(x => x.AccountInvoicePaymentRels).ToList());
        }

        public async Task CancelAsync(IEnumerable<AccountPayment> payments)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach (var rec in payments)
            {
                foreach (var move in rec.MoveLines.Select(x => x.Move).Distinct().ToList())
                {
                    if (rec.AccountInvoicePaymentRels.Any())
                        await moveLineObj.RemoveMoveReconcile(move.Lines.Select(x => x.Id).ToList());

                    await moveObj.ButtonCancel(new List<Guid>() { move.Id });
                    await moveObj.Unlink(new List<Guid>() { move.Id });
                }

                rec.State = "draft";
            }

            await UpdateAsync(payments);
        }

        public async Task UnlinkAsync(IEnumerable<Guid> ids)
        {
            var payments = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            await UnlinkAsync(payments);
        }

        public async Task UnlinkAsync(IEnumerable<AccountPayment> payments)
        {
            foreach (var rec in payments.ToList())
            {
                if (rec.State != "draft")
                    throw new Exception("Bạn không thể xóa thanh toán đã được vào sổ.");
            }

            await DeleteAsync(payments);
        }

        public async Task<IEnumerable<AccountPaymentBasic>> GetPaymentBasicList(AccountPaymentFilter val)
        {
            ISpecification<AccountPayment> spec = new InitialSpecification<AccountPayment>(x => true);

            if (val.JournalId.HasValue)
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.JournalId == val.JournalId));

            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PartnerId == val.PartnerId));

            if (!string.IsNullOrEmpty(val.PartnerType))
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PartnerType == val.PartnerType));

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                spec = spec.And(new InitialSpecification<AccountPayment>(x => states.Contains(x.State)));
            }

            if (val.DateFrom.HasValue)
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PaymentDate <= val.DateFrom));

            if (val.DateTo.HasValue)
            {
                var paymentDateTo = val.DateTo.Value.AddDays(1);
                spec = spec.And(new InitialSpecification<AccountPayment>(x => x.PaymentDate < paymentDateTo));
            }

            if (val.SaleOrderId.HasValue)
            {
                var orderObj = GetService<ISaleOrderService>();
                var order = orderObj.SearchQuery(x => x.Id == val.SaleOrderId)
                    .Include(x => x.OrderLines)
                    .Include("OrderLines.SaleOrderLineInvoiceRels")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice.AccountInvoicePaymentRels")
                    .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice.AccountInvoicePaymentRels.Payment")
                    .FirstOrDefault();
                var apIds = order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Select(x => x.InvoiceLine).Select(x => x.Invoice)
                    .SelectMany(x => x.AccountInvoicePaymentRels).Select(x => x.Payment);

                spec = spec.And(new InitialSpecification<AccountPayment>(x => apIds.Any(y => y.Id == x.Id)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<AccountPaymentBasic>(query).ToListAsync();

            return items;
        }

        public override ISpecification<AccountPayment> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.account_payment_comp_rule":
                    return new InitialSpecification<AccountPayment>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
