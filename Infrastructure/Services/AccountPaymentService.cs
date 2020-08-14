using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Data;
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
        private readonly CatalogDbContext _dbContext;

        public AccountPaymentService(IAsyncRepository<AccountPayment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, CatalogDbContext dbContext)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task Post(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.AccountMovePaymentRels)
                .Include(x => x.SaleOrderPaymentRels)
                .Include(x => x.CardOrderPaymentRels)
                .Include(x => x.Journal).Include(x => x.Journal.DefaultCreditAccount)
                .Include(x => x.Journal.DefaultDebitAccount).ToListAsync();

            var seqObj = GetService<IIRSequenceService>();
            var moveObj = GetService<IAccountMoveService>();

            foreach (var rec in self)
            {
                if (rec.State != "draft")
                    throw new Exception("Chỉ những thanh toán nháp mới được vào sổ.");

                if (await _dbContext.AccountMovePaymentRels.AnyAsync(x => x.PaymentId == rec.Id && x.Move.State != "posted"))
                    throw new Exception("The payment cannot be processed because the invoice is not open!");

                string sequence_code = "";
                if (string.IsNullOrEmpty(rec.Name))
                {
                    if (rec.PaymentType == "transfer")
                    {
                        sequence_code = "account.payment.transfer";
                    }
                    else
                    {
                        if (rec.PartnerType == "customer")
                        {
                            if (rec.PaymentType == "outbound")
                                sequence_code = "account.payment.customer.refund";
                            if (rec.PaymentType == "inbound")
                                sequence_code = "account.payment.customer.invoice";
                        }
                        if (rec.PartnerType == "supplier")
                        {
                            if (rec.PaymentType == "outbound")
                                sequence_code = "account.payment.supplier.invoice";
                            if (rec.PaymentType == "inbound")
                                sequence_code = "account.payment.supplier.refund";
                        }
                    }

                    rec.Name = await seqObj.NextByCode(sequence_code);

                    if (string.IsNullOrEmpty(rec.Name) && rec.PaymentType != "transfer")
                        throw new Exception($"You have to define a sequence for {sequence_code} in your company.");
                }

                var moves = await _PreparePaymentMoves(new List<AccountPayment>() { rec });

                var amlObj = GetService<IAccountMoveLineService>();
                foreach (var move in moves)
                    amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(moves);
                await moveObj.ActionPost(moves);

                foreach (var move in moves)
                    amlObj.ComputeMoveNameState(move.Lines);

                rec.State = "posted";
                await UpdateAsync(rec);

                if (rec.PaymentType == "inbound" || rec.PaymentType == "outbound")
                {
                    if (rec.AccountMovePaymentRels.Any())
                    {
                        var move_ids = rec.AccountMovePaymentRels.Select(x => x.MoveId);
                        var payment_moves = await moveObj.SearchQuery(x => move_ids.Contains(x.Id))
                            .Include(x => x.Lines).Include("Lines.Account").ToListAsync();

                        var invoices = moves.Concat(payment_moves);
                        await _AutoReconcile(rec, invoices);
                    }
                    else if (rec.SaleOrderPaymentRels.Any())
                    {
                        //tìm tất cả các moves của các sale orders
                        var sale_order_ids = rec.SaleOrderPaymentRels.Select(x => x.SaleOrderId).ToList();
                        var sale_moves = await moveObj.SearchQuery(x => x.Lines.Any(s => s.SaleLineRels.Any(m => sale_order_ids.Contains(m.OrderLine.OrderId))))
                            .Include(x => x.Lines).Include("Lines.Account").ToListAsync();

                        var invoices = moves.Concat(sale_moves);
                        await _AutoReconcile(rec, invoices);
                    }
                    else if (rec.CardOrderPaymentRels.Any())
                    {
                        //tìm tất cả các moves của các sale orders
                        var sale_order_ids = rec.CardOrderPaymentRels.Select(x => x.CardOrderId).ToList();
                        var sale_moves = await moveObj.SearchQuery(x => x.Lines.Any(s => s.CardOrderLineRels.Any(m => sale_order_ids.Contains(m.OrderLine.OrderId))))
                            .Include(x => x.Lines).Include("Lines.Account").ToListAsync();

                        var invoices = moves.Concat(sale_moves);
                        await _AutoReconcile(rec, invoices);

                        var cardOrderObj = GetService<IServiceCardOrderService>();
                        await cardOrderObj.UpdateResidual(sale_order_ids);
                    }
                    else
                    {
                        //auto reconcile theo partner
                        await _AutoReconcile(rec);
                    }
                }
            }
        }

        private async Task _AutoReconcile(AccountPayment self, IEnumerable<AccountMove> invoices = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var moveObj = GetService<IAccountMoveService>();
            IList<AccountMoveLine> lines = null;
            if (invoices == null)
            {
                lines = await amlObj.SearchQuery(x => !x.Reconciled && x.AccountId == self.DestinationAccount.Id && x.PartnerId == self.PartnerId,
                         orderBy: x => x.OrderBy(s => s.DateCreated))
                         .Include(x => x.Account).Include(x => x.MatchedDebits).Include(x => x.MatchedCredits).ToListAsync();
            }
            else
            {
                lines = invoices.SelectMany(x => x.Lines).Where(x => !x.Reconciled && x.AccountId == self.DestinationAccount.Id).ToList();
            }


            await amlObj.Reconcile(lines);

            amlObj._AmountResidual(lines);

            var move_ids = lines.Select(x => x.MoveId).Distinct().ToList();
            await moveObj._ComputeAmount(move_ids);

            //update sale order residual
            var saleLineObj = GetService<ISaleOrderLineService>();
            var saleOrderIds = await saleLineObj.SearchQuery(x => x.SaleOrderLineInvoice2Rels.Any(s => move_ids.Contains(s.InvoiceLine.Move.Id)))
                .Select(x => x.OrderId).Distinct().ToListAsync();
            if (saleOrderIds.Any())
            {
                var saleObj = GetService<ISaleOrderService>();
                await saleObj.RecomputeResidual(saleOrderIds);
            }
        }

        public async Task<IList<AccountMove>> _PreparePaymentMoves(IEnumerable<AccountPayment> self)
        {
            var all_move_vals = new List<AccountMove>();

            await _ComputeDestinationAccount(self);
            foreach (var payment in self)
            {
                decimal counterpart_amount = 0;
                AccountAccount liquidity_line_account = null;

                if (payment.PaymentType == "outbound" || payment.PaymentType == "transfer")
                {
                    counterpart_amount = payment.Amount;
                    liquidity_line_account = payment.Journal.DefaultDebitAccount;
                }
                else
                {
                    counterpart_amount = -payment.Amount;
                    liquidity_line_account = payment.Journal.DefaultCreditAccount;
                }

                var balance = counterpart_amount;
                var liquidity_amount = counterpart_amount;

                var rec_pay_line_name = "";
                if (payment.PaymentType == "transfer")
                    rec_pay_line_name = payment.Name;
                else
                {
                    if (payment.PartnerType == "customer")
                    {
                        if (payment.PaymentType == "inbound")
                            rec_pay_line_name += "Khách hàng thanh toán";
                        else if (payment.PaymentType == "outbound")
                            rec_pay_line_name += "Hoàn tiền khách hàng";
                    }
                    else if (payment.PartnerType == "supplier")
                    {
                        if (payment.PaymentType == "inbound")
                            rec_pay_line_name += "Nhà cung cấp hoàn tiền";
                        else if (payment.PaymentType == "outbound")
                            rec_pay_line_name += "Thanh toán nhà cung cấp";
                    }

                    if (payment.AccountMovePaymentRels.Any())
                    {
                        var moveObj = GetService<IAccountMoveService>();
                        var move_ids = payment.AccountMovePaymentRels.Select(x => x.MoveId);
                        var move_names = await moveObj.SearchQuery(x => move_ids.Contains(x.Id)).Select(x => x.Name).ToListAsync();
                        rec_pay_line_name += $": {string.Join(", ", move_names)}";
                    }
                }

                var liquidity_line_name = "";
                if (payment.PaymentType == "transfer")
                { }
                else
                    liquidity_line_name = payment.Name;

                var move_vals = new AccountMove
                {
                    Date = payment.PaymentDate,
                    Ref = payment.Communication,
                    JournalId = payment.JournalId,
                    Journal = payment.Journal,
                    PartnerId = payment.PartnerId,
                    CompanyId = payment.CompanyId,
                };

                var lines = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name = rec_pay_line_name,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        DateMaturity = payment.PaymentDate,
                        PartnerId = payment.PartnerId,
                        AccountId = payment.DestinationAccount.Id,
                        Account = payment.DestinationAccount,
                        PaymentId = payment.Id,
                        Move = move_vals,
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        DateMaturity = payment.PaymentDate,
                        PartnerId = payment.PartnerId,
                        AccountId = liquidity_line_account.Id,
                        Account = liquidity_line_account,
                        PaymentId = payment.Id,
                        Move = move_vals,
                    },
                };

                move_vals.Lines = lines;

                all_move_vals.Add(move_vals);
            }

            return all_move_vals;
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

            var payment_diff = _ComputePaymentDifference(new List<AccountPayment>() { rec })[rec.Id];
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

        public async Task<AccountRegisterPaymentDisplay> SaleDefaultGet(IEnumerable<Guid> saleOrderIds)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orders = await orderObj.SearchQuery(x => saleOrderIds.Contains(x.Id) && x.Residual > 0).ToListAsync();

            if (!orders.Any())
                throw new Exception("Phiếu điều trị đã thanh toán đủ");

            if (orders.Any(x => x.State != "sale" && x.State != "done"))
                throw new Exception("Bạn chỉ có thể thanh toán cho phiếu điều trị đã xác nhận");

            if (orders.Any(x => x.PartnerId != orders[0].PartnerId))
                throw new Exception("Để thanh toán nhiều phiếu điều trị cùng một lần, chúng phải có cùng khách hàng");

            var total_amount = orders.Sum(x => x.Residual);
            var paymentLines = await LinePaymentRelDefaultGet(orders[0].Id);

            var communication = string.Join(", ", orders.Select(x => x.Name));
            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount ?? 0),
                PaymentType = total_amount > 0 ? "inbound" : "outbound",
                PartnerId = orders[0].PartnerId,
                PartnerType = "customer",
                Communication = communication,
                SaleOrderIds = saleOrderIds,
                SaleOrderLinePaymentRels = paymentLines

            };

            return rec;
        }

        public async Task<IEnumerable<SaleOrderLinePaymentRelDisplay>> LinePaymentRelDefaultGet(Guid saleOrderId)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderlineObj = GetService<ISaleOrderLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var paymentRels = new List<SaleOrderLinePaymentRelDisplay>();
            var order = await orderObj.SearchQuery(x => x.Id == saleOrderId && x.Residual > 0).Include(x => x.OrderLines).FirstOrDefaultAsync();          
            foreach (var line in order.OrderLines)
            {
                var orline = _mapper.Map<SaleOrderLineDisplay>(line);
                var amountPrepaid = await linePaymentRelObj.SearchQuery(x => x.SaleOrderLineId == line.Id).SumAsync(x => x.AmountPrepaid.Value);
                var res = new SaleOrderLinePaymentRelDisplay
                {
                    SaleOrderLineId = orline.Id,
                    SaleOrderLine = orline,
                    isDone = amountPrepaid == line.PriceSubTotal ? true : false,
                    AmountPayment = await linePaymentRelObj.SearchQuery(x => x.SaleOrderLineId == line.Id).SumAsync(x => x.AmountPrepaid.Value),
                    AmountPrepaid = 0
                };
                paymentRels.Add(res);

                //if (res.AmountPayment != 0)
                //{
                //    line.AmountPaid = res.AmountPayment;
                //    line.AmountResidual = line.PriceSubTotal - res.AmountPayment;

                //    await orderlineObj.UpdateAsync(line);
                //}
   
            }

            return paymentRels;
        }
      

        public async Task<AccountRegisterPaymentDisplay> PurchaseDefaultGet(IEnumerable<Guid> purchaseOrderIds)
        {
            var orderObj = GetService<IPurchaseOrderService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var invoice_ids = await amlObj.SearchQuery(x => purchaseOrderIds.Contains(x.PurchaseLine.OrderId)).Select(x => x.MoveId).Distinct().ToListAsync();
            return await DefaultGet(invoice_ids);
        }

        public async Task<AccountRegisterPaymentDisplay> DefaultGet(IEnumerable<Guid> invoice_ids)
        {
            var amObj = GetService<IAccountMoveService>();
            var invoices = await amObj.SearchQuery(x => invoice_ids.Contains(x.Id)).ToListAsync();
            invoices = invoices.Where(x => amObj.IsInvoice(x, include_receipts: true)).ToList();

            if (!invoices.Any() || invoices.Any(x => x.State != "posted"))
                throw new Exception("You can only register payments for open invoices");
            var dtype = invoices[0].Type;
            foreach (var inv in invoices.Skip(1))
            {
                if (inv.Type != dtype)
                {
                    if ((dtype == "in_refund" && inv.Type == "in_invoice") || (dtype == "in_invoice" && inv.Type == "in_refund"))
                        throw new Exception("You cannot register payments for vendor bills and supplier refunds at the same time.");

                    if ((dtype == "out_refund" && inv.Type == "out_invoice") || (dtype == "out_invoice" && inv.Type == "out_refund"))
                        throw new Exception("You cannot register payments for customer invoices and credit notes at the same time.");
                }
            }

            var total_amount = invoices.Sum(x => x.AmountResidual * MAP_INVOICE_TYPE_PAYMENT_SIGN[x.Type]);
            var communication = !string.IsNullOrEmpty(invoices[0].InvoicePaymentRef) ? invoices[0].InvoicePaymentRef :
                (!string.IsNullOrEmpty(invoices[0].Ref) ? invoices[0].Ref : invoices[0].Name);

            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount ?? 0),
                PaymentType = total_amount > 0 ? "inbound" : "outbound",
                PartnerId = invoices[0].PartnerId,
                PartnerType = MAP_INVOICE_TYPE_PARTNER_TYPE[invoices[0].Type],
                //Communication = communication,
                InvoiceIds = invoice_ids
            };

            return rec;
        }

        public async Task<AccountRegisterPaymentDisplay> ServiceCardOrderDefaultGet(IEnumerable<Guid> order_ids)
        {
            var orderObj = GetService<IServiceCardOrderService>();
            var orders = await orderObj.SearchQuery(x => order_ids.Contains(x.Id) && x.AmountResidual > 0).ToListAsync();

            if (!orders.Any() || orders.Any(x => x.State != "sale" && x.State != "done"))
                throw new Exception("Bạn chỉ có thể thanh toán cho đơn hàng đã xác nhận");

            if (orders.Any(x => x.PartnerId != orders[0].PartnerId))
                throw new Exception("Để thanh toán nhiều đơn cùng một lần, chúng phải có cùng khách hàng");

            var total_amount = orders.Sum(x => x.AmountResidual);

            var communication = string.Join(", ", orders.Select(x => x.Name));
            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount ?? 0),
                PaymentType = total_amount > 0 ? "inbound" : "outbound",
                PartnerId = orders[0].PartnerId,
                PartnerType = "customer",
                Communication = communication,
                ServiceCardOrderIds = order_ids
            };

            return rec;
        }

        public async Task<AccountRegisterPaymentDisplay> PartnerDefaultGet(Guid partnerId)
        {
            //Tính số tiền còn nợ của partner
            var partnerObj = GetService<IPartnerService>();
            var creditDebitGet = partnerObj.CreditDebitGet(new List<Guid>() { partnerId });

            var partner = await partnerObj.GetByIdAsync(partnerId);
            decimal total_amount = 0;
            string partner_type = "";
            if (partner.Customer && partner.Supplier)
                throw new Exception("Không thể thanh toán cho đối tác vừa là khách hàng, vừa là nhà cung cấp");

            if (partner.Customer)
            {
                total_amount = creditDebitGet[partnerId].Credit;
                partner_type = "customer";
            }
            else if (partner.Supplier)
            {
                total_amount = creditDebitGet[partnerId].Debit;
                partner_type = "supplier";
            }

            if (total_amount == 0)
                throw new Exception("Không có gì để thanh toán");

            string payment_type = "";
            if (partner.Customer)
                payment_type = total_amount > 0 ? "inbound" : "outbound";
            else if (partner.Supplier)
                payment_type = total_amount > 0 ? "outbound" : "inbound";

            var rec = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(total_amount),
                PaymentType = payment_type,
                PartnerId = partnerId,
                PartnerType = partner_type,
            };

            return rec;
        }

        public async Task<AccountPayment> CreateUI(AccountPaymentSave val)
        {
            val.SaleOrderLinePaymentRels = val.SaleOrderLinePaymentRels.Where(x => x.AmountPrepaid > 0).ToList();
            var payment = _mapper.Map<AccountPayment>(val);
            var journalObj = GetService<IAccountJournalService>();
            var journal = await journalObj.GetByIdAsync(payment.JournalId);
            payment.CompanyId = journal.CompanyId;
            if (val.SaleOrderLinePaymentRels.Any())
            {
                if(val.Amount != val.SaleOrderLinePaymentRels.Sum(x => x.AmountPrepaid))
                {
                    throw new Exception("Thanh toán thất bại");
                }
            }

            foreach (var invoice_id in val.InvoiceIds)
                payment.AccountMovePaymentRels.Add(new AccountMovePaymentRel { MoveId = invoice_id });

            foreach (var order_id in val.SaleOrderIds)
                payment.SaleOrderPaymentRels.Add(new SaleOrderPaymentRel { SaleOrderId = order_id });


            foreach (var order_id in val.ServiceCardOrderIds)
                payment.CardOrderPaymentRels.Add(new ServiceCardOrderPaymentRel { CardOrderId = order_id });

            await CreateAsync(payment);

            if(payment.SaleOrderLinePaymentRels.Any())
            await _ComputeSaleOrderLines(val.SaleOrderIds.First());

            return payment;
        }

        public async Task _ComputeSaleOrderLines(Guid saleOrderId)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderlineObj = GetService<ISaleOrderLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var order = await orderObj.SearchQuery(x => x.Id == saleOrderId && x.Residual > 0).Include(x => x.OrderLines).FirstOrDefaultAsync();
            foreach (var line in order.OrderLines)
            {
                var amountPaid = await linePaymentRelObj.SearchQuery(x => x.SaleOrderLineId == line.Id).SumAsync(x => x.AmountPrepaid.Value);
                line.AmountPaid = amountPaid;
                line.AmountResidual = line.PriceSubTotal - amountPaid;                  
            }

            await orderlineObj.UpdateAsync(order.OrderLines);
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

        /// <summary>
        /// Thanh toán nợ, bắt đầu từ hóa đơn hết hạn sớm nhất của list phiếu điều trị truyền vào
        /// </summary>
        public void _ComputeResidualPayment(IEnumerable<SaleOrder> saleOrders, decimal amount)
        {
            foreach (var order in saleOrders)
            {
                var invoices = order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Select(x => x.InvoiceLine).Select(x => x.Invoice)
                    .OrderByDescending(x => x.DateDue).ToList();
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

            await _ComputeDestinationAccount(new List<AccountPayment>() { rec });
            var account = rec.DestinationAccount;
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

        public async Task _ComputeDestinationAccount(IEnumerable<AccountPayment> self)
        {
            foreach (var payment in self)
            {
                if (payment.AccountMovePaymentRels.Any())
                {
                    var amlObj = GetService<IAccountMoveLineService>();
                    var move_ids = payment.AccountMovePaymentRels.Select(x => x.MoveId).ToList();
                    payment.DestinationAccount = amlObj.SearchQuery(x => move_ids.Contains(x.MoveId))
                        .Include(x => x.Account).Select(x => x.Account).Where(x => x.InternalType == "receivable" || x.InternalType == "payable").FirstOrDefault();
                }
                else if (payment.PartnerType == "customer")
                {
                    var accountObj = GetService<IAccountAccountService>();
                    payment.DestinationAccount = await accountObj.GetAccountReceivableCurrentCompany();
                }
                else if (payment.PartnerType == "supplier")
                {
                    var accountObj = GetService<IAccountAccountService>();
                    payment.DestinationAccount = await accountObj.GetAccountPayableCurrentCompany();
                }
            }
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

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<AccountPaymentBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<AccountPaymentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task CancelAsync(IEnumerable<Guid> ids)
        {
            await CancelAsync(SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines)
                .Include("MoveLines.Move").Include("MoveLines.Move.Lines").Include(x => x.AccountMovePaymentRels).ToList());
        }

        public async Task<IEnumerable<AccountPayment>> ActionDraft(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines).Include(x=>x.SaleOrderPaymentRels).Include(x=>x.SaleOrderLinePaymentRels).ToListAsync();
            var moveObj = GetService<IAccountMoveService>();
            var move_ids = self.SelectMany(x => x.MoveLines).Select(x => x.MoveId).Distinct().ToList();
            var moves = await moveObj.ButtonDraft(move_ids);

            await moveObj.Unlink(move_ids);

          
            foreach (var rec in self)
            {               
                

                rec.State = "draft";
            }
               
            await UpdateAsync(self);
            return self;
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

        public async Task ActionDraftUnlink(IEnumerable<Guid> ids)
        {
            var self = await ActionDraft(ids);
            await UnlinkAsync(ids);
        }

        public async Task UnlinkAsync(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines).ToListAsync();
            foreach (var rec in self)
            {
               
                if (rec.MoveLines.Any())
                    throw new Exception("Bạn không thể xóa thanh toán đã được vào sổ.");
            }

            await DeleteAsync(self);

            ///update saleorderline
            foreach(var rec in self)
            await _ComputeSaleOrderLines(rec.SaleOrderPaymentRels.Select(x => x.SaleOrderId).First());
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
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.account_payment_comp_rule":
                    return new InitialSpecification<AccountPayment>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        public async Task<AccountPaymentPrintVM> GetPrint(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id).Select(x => new AccountPaymentPrintVM
            {
                CompanyName = x.Company.Name,
                CompanyStreet = x.Company.Partner.Street,
                CompanyCity = x.Company.Partner.CityName,
                CompanyDistrict = x.Company.Partner.DistrictName,
                CompanyWard = x.Company.Partner.WardName,
                Amount = x.Amount,
                Communication = x.Communication,
                CompanyEmail = x.Company.Email,
                CompanyPhone = x.Company.Phone,
                JournalName = x.Journal.Name,
                PartnerCity = x.Partner.CityName,
                PartnerDistrict = x.Partner.DistrictName,
                PartnerName = x.Partner.Name,
                PartnerDisplayName = x.Partner.DisplayName,
                PartnerPhone = x.Partner.Phone,
                PartnerRef = x.Partner.Ref,
                PartnerStreet = x.Partner.Street,
                PartnerWard = x.Partner.WardName,
                PaymentDate = x.PaymentDate,
                SaleOrders = x.SaleOrderPaymentRels.Select(s => new AccountPaymentSaleOrderPrintVM
                {
                    AmountTotal = s.SaleOrder.AmountTotal,
                    DateOrder = s.SaleOrder.DateOrder,
                    Name = s.SaleOrder.Name,
                    Residual = s.SaleOrder.Residual
                }),
            }).FirstOrDefaultAsync();

            if (res == null)
                throw new Exception("Null payment");

            var tmp = new List<string>();
            if (!string.IsNullOrEmpty(res.CompanyStreet))
                tmp.Add(res.CompanyStreet);
            if (!string.IsNullOrEmpty(res.CompanyWard))
                tmp.Add(res.CompanyWard);
            if (!string.IsNullOrEmpty(res.CompanyDistrict))
                tmp.Add(res.CompanyDistrict);
            if (!string.IsNullOrEmpty(res.CompanyCity))
                tmp.Add(res.CompanyCity);

            res.CompanyAddress = string.Join(", ", tmp);

            var tmp2 = new List<string>();
            if (!string.IsNullOrEmpty(res.PartnerStreet))
                tmp2.Add(res.PartnerStreet);
            if (!string.IsNullOrEmpty(res.PartnerWard))
                tmp2.Add(res.PartnerWard);
            if (!string.IsNullOrEmpty(res.PartnerDistrict))
                tmp2.Add(res.PartnerDistrict);
            if (!string.IsNullOrEmpty(res.PartnerCity))
                tmp2.Add(res.PartnerCity);

            res.PartnerAddress = string.Join(", ", tmp2);

            return res;
        }

    }


}
