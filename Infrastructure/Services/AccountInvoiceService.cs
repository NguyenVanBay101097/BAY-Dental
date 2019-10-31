using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountInvoiceService : BaseService<AccountInvoice>, IAccountInvoiceService
    {
        private readonly IMapper _mapper;
        private readonly IProductStepService _productStepService;
        public AccountInvoiceService(IAsyncRepository<AccountInvoice> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProductStepService productStepService)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _productStepService = productStepService;
        }

        public static IDictionary<string, string> TYPE2JOURNAL
        {
            get
            {
                var res = new Dictionary<string, string>();
                res.Add("out_invoice", "sale");
                res.Add("in_invoice", "purchase");
                res.Add("out_refund", "sale");
                res.Add("in_refund", "purchase");
                return res;
            }
        }

        public async Task DeleteInvoice(Guid id)
        {
            var invoice = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            await DeleteAsync(invoice);
        }

        public override async Task DeleteAsync(IEnumerable<AccountInvoice> self)
        {
            foreach (var invoice in self)
            {
                if (invoice.State != "draft" && invoice.State != "cancel")
                    throw new Exception("Bạn chỉ có thể xóa hóa đơn có trạng thái mới hoặc hủy bỏ.");
                else if (!string.IsNullOrEmpty(invoice.MoveName))
                    throw new Exception("Bạn không thể xóa hóa đơn đã xác nhận (có số hóa đơn). Bạn có thể chuyển sang trạng thái nháp, thay đổi nội dung hóa đơn sau đó xác nhận lại.");
            }

            await base.DeleteAsync(self);
        }

        public async Task<IEnumerable<PaymentInfoContent>> _GetPaymentInfoJson(Guid id)
        {
            var invoice = await SearchQuery(x => x.Id == id).Include(x => x.PaymentMoveLines)
                .Include("PaymentMoveLines.MoveLine")
                .Include("PaymentMoveLines.MoveLine.Move")
                .Include("PaymentMoveLines.MoveLine.Journal")
                .Include("PaymentMoveLines.MoveLine.MatchedDebits")
                .Include("PaymentMoveLines.MoveLine.MatchedCredits")
                .Include(x => x.Move).Include("Move.Lines")
                .FirstOrDefaultAsync();
            var res = new List<PaymentInfoContent>();
            foreach (var rel in invoice.PaymentMoveLines)
            {
                var payment = rel.MoveLine;
                decimal amount = 0;
                if (invoice.Type == "out_invoice" || invoice.Type == "in_refund")
                {
                    amount = payment.MatchedDebits.Where(x => invoice.Move.Lines.Contains(x.DebitMove)).Sum(x => x.Amount);
                }
                else if (invoice.Type == "in_invoice" || invoice.Type == "out_refund")
                {
                    amount = payment.MatchedCredits.Where(x => invoice.Move.Lines.Contains(x.CreditMove)).Sum(x => x.Amount);
                }

                if (amount == 0)
                    continue;

                var paymentRef = payment.Move.Name;
                if (!string.IsNullOrEmpty(payment.Move.Ref))
                    paymentRef += " (" + payment.Move.Ref + ")";

                res.Add(new PaymentInfoContent
                {
                    Name = payment.Name,
                    JournalName = payment.Journal.Name,
                    Amount = amount,
                    Date = payment.Date,
                    Ref = paymentRef,
                    PaymentId = payment.Id,
                    MoveId = payment.MoveId,
                    AccountPaymentId = payment.PaymentId,
                });
            }

            return res;
        }

        public async Task<AccountInvoiceDisplay> DefaultGet(AccountInvoiceDisplay val)
        {
            var res = new AccountInvoiceDisplay();
            res.CompanyId = CompanyId;
            res.UserId = UserId;
            res.Type = val.Type;
            var journal = await _DefaultJournal(val.Type);
            if (journal != null)
            {
                res.JournalId = journal.Id;
            }

            AccountAccount account = null;
            var type = val.Type;
            var accountService = (IAccountAccountService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountAccountService));
            if (type == "out_invoice" || type == "out_refund")
            {
                account = await accountService.GetAccountReceivableCurrentCompany();
            }
            else
            {
                account = await accountService.GetAccountPayableCurrentCompany();
            }

            if (account == null)
                throw new Exception("Không tìm thấy tài khoản kế toán cho công ty này");
            res.AccountId = account.Id;

            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            return res;
        }

        public override async Task UpdateAsync(IEnumerable<AccountInvoice> self)
        {
            var pre_not_reconciled = self.Where(x => x.Reconciled == false);
            var pre_reconciled = self.Except(pre_not_reconciled);
            await base.UpdateAsync(self);

            var reconciled = self.Where(x => x.Reconciled == true);
            var not_reconciled = self.Except(reconciled);
            await ActionInvoicePaid(reconciled.Union(pre_reconciled).Where(x => x.State == "open").ToList());
            await ActionInvoiceReOpen(not_reconciled.Union(pre_not_reconciled).Where(x => x.State == "paid").ToList());
        }

        private async Task ActionInvoiceReOpen(IEnumerable<AccountInvoice> self)
        {
            if (self.Any(x => x.State != "paid"))
                throw new Exception("Hóa đơn đã được trả để được thanh toán");
            foreach (var inv in self)
                inv.State = "open";
            await base.UpdateAsync(self);
        }

        private async Task ActionInvoicePaid(IEnumerable<AccountInvoice> self)
        {
            var to_pay_invoices = self.Where(x => x.State != "paid").ToList();
            if (to_pay_invoices.Any(x => x.State != "open"))
                throw new Exception("Hóa đơn phải được xác nhận để được thanh toán");
            if (to_pay_invoices.Any(x => x.Reconciled == false))
                throw new Exception("Bạn cần đối soát những thanh toán cho hóa đơn");
            foreach (var inv in to_pay_invoices)
                inv.State = "paid";
            await base.UpdateAsync(to_pay_invoices);
        }

        private async Task<AccountJournal> _DefaultJournal(string invType)
        {
            var type = TYPE2JOURNAL[invType];
            var companyId = CompanyId;
            var journalService = (IAccountJournalService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountJournalService));
            var res = await journalService.GetJournalByTypeAndCompany(type, companyId);
            return res;
        }

        public async Task<AccountInvoice> GetAccountInvoiceForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.InvoiceLines)
                .Include("InvoiceLines.Product")
                .Include("InvoiceLines.ToothCategory")
                .Include("InvoiceLines.AccountInvoiceLineToothRels")
                .Include("InvoiceLines.AccountInvoiceLineToothRels.Tooth")
                .FirstOrDefaultAsync();
        }

        public async Task<AccountInvoicePrint> GetAccountInvoicePrint(Guid id)
        {
            var invoice = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Company)
                .Include(x => x.Company.Partner)
                .Include(x => x.InvoiceLines)
                .Include("InvoiceLines.Product")
                .FirstOrDefaultAsync();
            var res = _mapper.Map<AccountInvoicePrint>(invoice);
            var partnerObj = GetService<IPartnerService>();
            res.CompanyAddress = partnerObj.GetFormatAddress(invoice.Company.Partner);
            res.PartnerAddress = partnerObj.GetFormatAddress(invoice.Partner);
            return res;
        }

        public override Task<AccountInvoice> CreateAsync(AccountInvoice entity)
        {
            var invLineObj = GetService<IAccountInvoiceLineService>();
            invLineObj.UpdateRelatedData(entity.InvoiceLines, entity);
            invLineObj.ComputePrice(entity.InvoiceLines);
            _ComputeAmount(entity);
            return base.CreateAsync(entity);
        }

        public async Task<IEnumerable<AccountInvoiceCbx>> GetOpenPaid(string search = "")
        {
            var states = new string[] { "open", "paid" };
            var res = await SearchQuery(x => (string.IsNullOrEmpty(search) || x.Number.Contains(search)) && x.State != "draft",
                orderBy: x => x.OrderBy(s => s.Number), limit: 10)
                .Select(x => new AccountInvoiceCbx
                {
                    Id = x.Id,
                    Number = x.Number
                }).ToListAsync();
            return res;
        }

        public void _ComputeAmount(AccountInvoice invoice)
        {
            invoice.AmountUntaxed = invoice.InvoiceLines.Any() ? invoice.InvoiceLines.Sum(x => x.PriceSubTotal) : 0;
            if (invoice.DiscountType == "percentage")
                invoice.DiscountAmount = Math.Round(invoice.AmountUntaxed * invoice.DiscountPercent / 100);
            else
                invoice.DiscountAmount = invoice.DiscountFixed;

            invoice.AmountUntaxed = invoice.AmountUntaxed - invoice.DiscountAmount;
            invoice.AmountTax = 0;
            invoice.AmountTotal = invoice.AmountUntaxed + invoice.AmountTax;

            var sign = invoice.Type == "in_refund" || invoice.Type == "out_refund" ? -1 : 1;
            invoice.AmountTotalSigned = invoice.AmountTotal * sign;
        }

        public void _ComputeAmount(IEnumerable<AccountInvoice> self)
        {
            foreach(var invoice in self)
            {
                invoice.AmountUntaxed = invoice.InvoiceLines.Any() ? invoice.InvoiceLines.Sum(x => x.PriceSubTotal) : 0;
                if (invoice.DiscountType == "percentage")
                    invoice.DiscountAmount = Math.Round(invoice.AmountUntaxed * invoice.DiscountPercent / 100);
                else
                    invoice.DiscountAmount = invoice.DiscountFixed;

                invoice.AmountUntaxed = invoice.AmountUntaxed - invoice.DiscountAmount;
                invoice.AmountTax = 0;
                invoice.AmountTotal = invoice.AmountUntaxed + invoice.AmountTax;

                var sign = invoice.Type == "in_refund" || invoice.Type == "out_refund" ? -1 : 1;
                invoice.AmountTotalSigned = invoice.AmountTotal * sign;
            }
        }

        public async Task ActionInvoiceOpen(IEnumerable<Guid> ids)
        {
            //lots of duplicate calls to action_invoice_open, so we remove those already open
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move).Include(x => x.Journal).Include(x => x.InvoiceLines)
                .Include("InvoiceLines.Account").Include("InvoiceLines.Product").Include(x => x.Account)
                .Include(x => x.Partner).Include(x => x.Company).Include(x => x.Journal.Sequence)
                .ToListAsync();
            var to_open_invoices = self.Where(x => x.State != "open").ToList();
            if (to_open_invoices.Any(x => x.State != "draft"))
                throw new Exception("Hóa đơn phải ở trạng thái nháp mới có thể xác nhận.");
            if (to_open_invoices.Any(x => x.AmountTotal < 0))
                throw new Exception("Bạn không thể xác nhận hóa đơn với tổng tiền âm.");
            if (to_open_invoices.Any(x => !x.AccountId.HasValue))
                throw new Exception("No account was found to create the invoice, be sure you have installed a chart of account.");

            ActionDateAssign(to_open_invoices);

            await ActionMoveCreate(to_open_invoices);

            //await ActionDotKhamCreate(to_open_invoices);

            //await _GenerateDotKhamSteps(to_open_invoices);

            await InvoiceValidate(to_open_invoices);
        }

        private async Task _GenerateDotKhamSteps(IEnumerable<AccountInvoice> self)
        {
            var dotKhamStepService = GetService<IDotKhamStepService>();
            foreach (var invoice in self)
            {
                foreach (var line in invoice.InvoiceLines)
                {
                    if (!line.ProductId.HasValue || line.Product == null)
                        continue;

                    var steps = await _productStepService.SearchQuery(x => x.ProductId == line.ProductId).ToListAsync();
                    var list = new List<DotKhamStep>();
                    if (steps.Any())
                    {
                        foreach (var step in steps)
                        {
                            if (step.Default == true)
                            {
                                list.Add(new DotKhamStep
                                {
                                    Name = step.Name,
                                    InvoicesId = invoice.Id,
                                    ProductId = line.ProductId.Value,
                                    Order = step.Order
                                });
                            }
                        }
                    }
                    else
                    {
                        list.Add(new DotKhamStep
                        {
                            Name = line.Product.Name,
                            InvoicesId = invoice.Id,
                            ProductId = line.ProductId.Value,
                            Order = 0
                        });
                    }
                    await dotKhamStepService.CreateAsync(list);
                }
            }
        }

        private async Task ActionDotKhamCreate(IEnumerable<AccountInvoice> self)
        {
            var dotKhamObj = GetService<IDotKhamService>();
            foreach (var invoice in self)
            {
                await dotKhamObj.CreateAsync(new DotKham
                {
                    InvoiceId = invoice.Id,
                    PartnerId = invoice.PartnerId,
                    CompanyId = invoice.CompanyId,
                    UserId = invoice.UserId,
                });
            }
        }

        private async Task InvoiceValidate(IEnumerable<AccountInvoice> self)
        {
            foreach (var invoice in self)
            {
                invoice.State = "open";
            }
            await UpdateAsync(self);
        }

        private async Task ActionMoveCreate(IEnumerable<AccountInvoice> self)
        {
            //Creates invoice related analytics and financial move lines
            var accountMoveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach (var invoice in self)
            {
                if (invoice.Journal.Sequence == null)
                    throw new Exception("Vui lòng xác nhận sổ nhật ký cho hóa đơn này.");

                if (!invoice.InvoiceLines.Any())
                    throw new Exception("Bạn chưa thêm chi tiết hóa đơn nào.");

                if (invoice.Move != null)
                    return;

                if (!invoice.DateInvoice.HasValue)
                    invoice.DateInvoice = DateTime.Today;

                if (!invoice.DateDue.HasValue)
                    invoice.DateDue = invoice.DateInvoice;

                var dateInvoice = invoice.DateInvoice;
                var iml = InvoiceLineMoveLineGet(invoice);
                var date = dateInvoice;

                var computeInvoiceRes = ComputeInvoiceTotals(invoice, iml);
                var total = computeInvoiceRes.Total;
                iml = computeInvoiceRes.InvoiceMoveLines;

                var name = !string.IsNullOrEmpty(invoice.Name) ? invoice.Name : "/";
                iml.Add(new AccountMoveLineItem
                {
                    Type = "dest",
                    Name = name,
                    Price = total,
                    Account = invoice.Account,
                    AccountId = invoice.AccountId.Value,
                    DateMaturity = invoice.DateDue,
                    InvoiceId = invoice.Id,
                });

                var part = invoice.Partner;
                if (part == null)
                    throw new Exception("Không tìm thấy đối tác ghi sổ.");
                var lines = new List<AccountMoveLine>();
                foreach (var l in iml)
                {
                    lines.Add(LineGetConvert(l, part.Id, date.Value, invoice.JournalId));
                }

                //tạo account move: 1 dòng trong nhật ký
                var move_vals = new AccountMove()
                {
                    Ref = invoice.Reference,
                    JournalId = invoice.JournalId,
                    Journal = invoice.Journal,
                    Date = invoice.DateInvoice.Value,
                    Narration = invoice.Comment,
                    CompanyId = invoice.Journal.CompanyId,
                    Company = invoice.Journal.Company,
                    PartnerId = invoice.PartnerId,
                    Lines = lines
                };

                await accountMoveObj.CreateAsync(move_vals);
                await accountMoveObj.Post(new List<AccountMove>() { move_vals }, invoice: invoice);

                invoice.MoveId = move_vals.Id;
                invoice.Move = move_vals;
                invoice.Date = move_vals.Date;
                invoice.MoveName = move_vals.Name;
                invoice.Number = move_vals.Name;

                _ComputeResidual(new List<AccountInvoice>() { invoice });
                _ComputePayments(new List<AccountInvoice>() { invoice });
                await UpdateAsync(invoice);
            }
        }

        /// <summary>
        /// 'state', 'currency_id', 'invoice_line_ids.price_subtotal',
        /// 'move_id.line_ids.amount_residual',
        /// 'move_id.line_ids.currency_id')
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<AccountInvoice> _ComputeResidual(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Move).Include("Move.Lines")
                .Include("Move.Lines.Account").ToList();
            return _ComputeResidual(self);
        }

        public IEnumerable<AccountInvoice> _ComputeResidual(IEnumerable<AccountInvoice> self)
        {
            foreach (var invoice in self)
            {
                decimal residual = 0;
                decimal residualCompanySigned = 0;
                var sign = invoice.Type == "in_refund" || invoice.Type == "out_refund" ? -1 : 1;


                invoice.Residual = 0;
                var partialReconciliationsDone = new List<long>();

                if (invoice.Move != null)
                {
                    foreach (var line in invoice.Move.Lines)
                    {
                        if (line.Account.InternalType != "receivable" && line.Account.InternalType != "payable")
                            continue;

                        residualCompanySigned += line.AmountResidual;
                        residual += line.AmountResidual;
                    }
                }

                invoice.ResidualSigned = Math.Abs(residual) * sign;
                invoice.Residual = Math.Abs(residual);
                if (invoice.Residual == 0)
                    invoice.Reconciled = true;
                else
                    invoice.Reconciled = false;
            }

            return self;
        }

        public async Task UpdateResidual(IEnumerable<Guid> ids)
        {
            var invoices = _ComputeResidual(ids);
            await UpdateAsync(invoices);
        }

        private AccountMoveLine LineGetConvert(AccountMoveLineItem line, Guid partnerId, DateTime date, Guid journalId)
        {
            return new AccountMoveLine()
            {
                PartnerId = partnerId,
                Name = line.Name,
                Date = date,
                Debit = line.Price > 0 ? line.Price : 0,
                Credit = line.Price < 0 ? -line.Price : 0,
                Quantity = line.Quantity,
                ProductId = line.ProductId,
                ProductUoMId = line.UoMId,
                JournalId = journalId,
                Account = line.Account,
                DateMaturity = line.DateMaturity ?? date,
                AccountId = line.AccountId,
                CompanyId = line.Account.CompanyId,
                Company = line.Account.Company,
                InvoiceId = line.InvoiceId,
            };
        }

        private ComputeInvoiceTotalsRes ComputeInvoiceTotals(AccountInvoice invoice, IList<AccountMoveLineItem> invoiceMoveLines)
        {
            decimal total = 0;
            foreach (var line in invoiceMoveLines)
            {
                line.Price = Math.Round(line.Price);

                if (invoice.Type == "out_invoice" || invoice.Type == "in_refund")
                {
                    total += line.Price;
                    line.Price = -line.Price;
                }
                else
                {
                    total -= line.Price;
                }
            }

            return new ComputeInvoiceTotalsRes
            {
                Total = total,
                InvoiceMoveLines = invoiceMoveLines
            };
        }

        private IList<AccountMoveLineItem> InvoiceLineMoveLineGet(AccountInvoice invoice)
        {
            var res = new List<AccountMoveLineItem>();
            foreach (var line in invoice.InvoiceLines)
            {
                var moveLineDict = new AccountMoveLineItem
                {
                    InvlId = line.Id,
                    Type = "src",
                    Name = line.Name,
                    PriceUnit = line.PriceUnit,
                    Quantity = line.Quantity,
                    Price = line.PriceSubTotal,
                    Account = line.Account,
                    AccountId = line.AccountId,
                    ProductId = line.ProductId,
                    UoMId = line.UoMId,
                    InvoiceId = invoice.Id,
                };

                res.Add(moveLineDict);
            }

            if (invoice.DiscountAmount > 0 && res.Any())
            {
                var firstLine = res.First();
                res.Add(new AccountMoveLineItem
                {
                    Type = "src",
                    Name = "Giảm tiền tổng hóa đơn",
                    PriceUnit = -invoice.DiscountAmount,
                    Quantity = 1,
                    Price = -invoice.DiscountAmount,
                    Account = firstLine.Account,
                    AccountId = firstLine.AccountId,
                    InvoiceId = invoice.Id,
                });
            }

            return res;
        }

        private void ActionDateAssign(IEnumerable<AccountInvoice> self)
        {
            foreach (var inv in self)
            {
                _OnChangePaymentTermDateInvoice(inv);
            }
        }

        private void _OnChangePaymentTermDateInvoice(AccountInvoice self)
        {
            var date_invoice = self.DateInvoice;
            if (date_invoice.HasValue)
                date_invoice = DateTime.Today;
            //if (self.PaymentTerm == null)
            //{
            //    self.DateDue = self.DateDue ?? self.DateInvoice;
            //}
            //else
            //{
            //    var pterm = self.PaymentTerm;
            //    var pterm_list = pterm.Compute(1, date_ref: date_invoice, currency: self.Company.Currency);
            //    self.DateDue = pterm_list.Max(x => x.Date);
            //}
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
             .Include(x => x.Move).Include(x => x.PaymentMoveLines)
             .Include("PaymentMoveLines.MoveLine")
             .Include("PaymentMoveLines.MoveLine.Payment")
             .Include(x => x.Move.Company).Include(x => x.Move.Journal)
             .Include(x => x.Move.Lines)
             .ToListAsync();

            var moves = new List<AccountMove>();
            foreach (var inv in self)
            {
                if (inv.Move != null)
                    moves.Add(inv.Move);
                if (inv.PaymentMoveLines.Any())
                {
                    var payments = inv.PaymentMoveLines.Select(x => x.MoveLine).Where(x => x.Payment != null).Select(x => x.Payment).Distinct().ToList();
                    throw new Exception("Bạn không thể hủy bỏ phiếu điều trị đã trả một phần, bạn cần hủy những dòng chi trả trước: " +
                        string.Join(", ", payments.Select(x => x.Name)));
                }
                   
                if (inv.DotKhams.Any())
                    throw new Exception("Bạn không thể hủy phiếu điều trị đã tạo đợt khám.");

                inv.Move = null;
                inv.State = "cancel";
            }

            await UpdateAsync(self);

            if (moves.Any())
            {
                var moveObj = GetService<IAccountMoveService>();
                await moveObj.ButtonCancel(moves);
                await moveObj.Unlink(moves);
            }
        }

        public async Task ActionCancelDraft(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var inv in self)
            {
                inv.State = "draft";
            }
            await UpdateAsync(self);
        }

        public async Task RegisterPayment(IEnumerable<Guid> invoiceIds, AccountMoveLine paymentLine)
        {
            var invoices = await SearchQuery(x => invoiceIds.Contains(x.Id))
                .Include(x => x.Move).Include(x => x.Move.Lines)
                .Include(x => x.PaymentMoveLines)
                .Include("Move.Lines.MatchedDebits").Include("Move.Lines.MatchedCredits")
                .Include("Move.Lines.Account")
                .ToListAsync();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var invObj = GetService<IAccountInvoiceService>();
            var lineToReconcile = new List<AccountMoveLine>();
            foreach (var inv in invoices)
            {
                lineToReconcile.AddRange(inv.Move.Lines.Where(x => x.Reconciled == false && (x.Account.InternalType == "payable" || x.Account.InternalType == "receivable")));
            }

            var reconcileLines = lineToReconcile.Concat(new List<AccountMoveLine>() { paymentLine });
            await moveLineObj.Reconcile(reconcileLines.ToList());

            moveLineObj._AmountResidual(reconcileLines);
            await moveLineObj.UpdateAsync(reconcileLines);

            //update invoices lại
            _ComputeResidual(invoices);
            _ComputePayments(invoices);
            await UpdateAsync(invoices);
        }

        public async Task UpdatePayments(IEnumerable<Guid> ids)
        {
            var invoices = _ComputePayments(ids);
            await UpdateAsync(invoices);
        }

        public IEnumerable<AccountInvoice> _ComputePayments(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Move).Include("Move.Lines")
                .Include("Move.Lines.MatchedCredits").Include("Move.Lines.MatchedDebits").ToList();
            return _ComputePayments(self);
        }

        /// <summary>
        /// move_id.line_ids.amount_residual
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public IEnumerable<AccountInvoice> _ComputePayments(IEnumerable<AccountInvoice> self)
        {
            foreach (var invoice in self)
            {
                var payment_lines = new List<Guid>();
                if (invoice.Move != null)
                {
                    foreach (var line in invoice.Move.Lines)
                    {
                        payment_lines.AddRange(line.MatchedCredits.Select(x => x.CreditMoveId));
                        payment_lines.AddRange(line.MatchedDebits.Select(x => x.DebitMoveId));
                    }
                }

                invoice.PaymentMoveLines.Clear();
                foreach (var item in payment_lines.Distinct().ToList())
                {
                    invoice.PaymentMoveLines.Add(new AccountInvoiceAccountMoveLineRel
                    {
                        AccountInvoiceId = invoice.Id,
                        MoveLineId = item
                    });
                }
            }

            return self;
        }

        public async Task<PagedResult<AccountInvoice>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20)
        {
            var items = await SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number), offSet: pageIndex * pageSize, limit: pageSize)
                .Include(x => x.Partner).Include(x => x.User)
                .ToListAsync();
            var totalItems = await SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number)).CountAsync();

            return new PagedResult<AccountInvoice>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<AccountInvoiceBasic>> GetPagedResultAsync(AccountInvoicePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.SearchPartnerNamePhone))
                query = query.Where(x => x.Partner.Name.Contains(val.SearchPartnerNamePhone) || x.Partner.Phone.Contains(val.SearchPartnerNamePhone));
            if (!string.IsNullOrEmpty(val.SearchNumber))
                query = query.Where(x => x.Number.Contains(val.SearchNumber));
            if (val.DateInvoiceFrom.HasValue)
                query = query.Where(x => x.DateInvoice >= val.DateInvoiceFrom);
            if (val.DateInvoiceTo.HasValue)
                query = query.Where(x => x.DateInvoice <= val.DateInvoiceTo);
            if (val.DateOrderFrom.HasValue)
                query = query.Where(x => x.DateOrder >= val.DateOrderFrom);
            if (val.DateOrderTo.HasValue)
            {
                var dateOrderTo = val.DateOrderTo.Value.AddDays(1);
                query = query.Where(x => x.DateOrder < dateOrderTo);
            }
            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                query = query.Where(x => types.Contains(x.Type));
            }
            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }
            if (!string.IsNullOrEmpty(val.UserId))
            {
                query = query.Where(x => x.UserId == val.UserId);
            }
            if (val.PartnerId.HasValue)
            {
                query = query.Where(x => x.PartnerId == val.PartnerId);
            }

            query = query.OrderByDescending(s => s.DateOrder);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .Include(x => x.Partner).Include(x => x.User)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<AccountInvoiceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AccountInvoiceBasic>>(items)
            };
        }

        public async Task UpdateInvoice(AccountInvoice inv)
        {
            var invLineObj = GetService<IAccountInvoiceLineService>();
            invLineObj.UpdateRelatedData(inv.InvoiceLines, inv);
            invLineObj.ComputePrice(inv.InvoiceLines);
            _ComputeAmount(inv);
            await base.UpdateAsync(inv);
        }

        public async Task TriggerChange_MoveLinesResidual(IEnumerable<AccountMoveLine> amls)
        {
            var invs = amls.Where(x => x.Invoice != null).Select(x => x.Invoice).Distinct().ToList();
            if (invs.Any())
            {
                _ComputeResidual(invs);
                _ComputePayments(invs);
                await UpdateAsync(invs);
            }
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var invoice in self)
            {
                if (invoice.State != "draft" && invoice.State != "cancel")
                    throw new Exception("Bạn không thể xóa hóa đơn có trạng thái khác mới hoặc hủy bỏ.");
                else if (!string.IsNullOrEmpty(invoice.MoveName))
                    throw new Exception("Bạn không thể xóa hóa đơn đã xác nhận (có số hóa đơn). Bạn có thể chuyển sang trạng thái nháp, thay đổi nội dung hóa đơn sau đó xác nhận lại.");
            }

            await DeleteAsync(self);
        }

        public override ISpecification<AccountInvoice> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.invoice_comp_rule":
                    return new InitialSpecification<AccountInvoice>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<AccountInvoiceLine> PrepareInvoiceLineFromLBLine(AccountInvoice self, LaboOrderLine line, AccountJournal journal = null, string type = "out_invoice")
        {
            var invLineObj = GetService<IAccountInvoiceLineService>();
            decimal qty = line.ProductQty - line.QtyInvoiced;
            if (qty <= 0)
                qty = 0;

            var account = await invLineObj._DefaultAccount(journal: journal, type: type);
            if (account == null)
                throw new Exception("Không tìm thấy tài khoản cho chi tiết hóa đơn");
            var data = new AccountInvoiceLine
            {
                LaboLineId = line.Id,
                Name = line.Order.Name + ": " + line.Name,
                UoMId = line.Product.UOMId,
                ProductId = line.ProductId,
                AccountId = account.Id,
                PriceUnit = line.PriceUnit,
                Quantity = qty,
                Discount = 0,
                InvoiceId = self.Id,
                Invoice = self,
            };

            return data;
        }
    }

    public class ComputeInvoiceTotalsRes
    {
        public decimal Total { get; set; }

        public IList<AccountMoveLineItem> InvoiceMoveLines { get; set; }
    }

    public class AccountMoveLineItem
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public Guid? ProductId { get; set; }
        public Guid? UoMId { get; set; }

        public Guid? InvlId { get; set; }

        public AccountAccount Account { get; set; }
        public Guid AccountId { get; set; }

        public Guid InvoiceId { get; set; }

        public DateTime? DateMaturity { get; set; }
    }

    public class PaymentInfoContent
    {
        public string Name { get; set; }

        public string JournalName { get; set; }

        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }

        public Guid PaymentId { get; set; }

        public Guid MoveId { get; set; }

        public string Ref { get; set; }

        public Guid? AccountPaymentId { get; set; }

        public string PaymentPartnerType { get; set; }
    }
}
