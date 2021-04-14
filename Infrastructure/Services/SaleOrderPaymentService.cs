using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class SaleOrderPaymentService : BaseService<SaleOrderPayment>, ISaleOrderPaymentService
    {
        private readonly IMapper _mapper;

        public SaleOrderPaymentService(IAsyncRepository<SaleOrderPayment> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderPaymentBasic>> GetPagedResultAsync(SaleOrderPaymentPaged val)
        {
            var query = SearchQuery();


            if (val.SaleOrderId.HasValue)
            {
                query = query.Where(x => x.OrderId == val.SaleOrderId.Value);
            }


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<SaleOrderPaymentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPaymentBasic>>(items)
            };

            return paged;
        }

        public async Task<SaleOrderPaymentDisplay> GetDisplay(Guid id)
        {
            var paymentJournalLineObj = GetService<ISaleOrderPaymentJournalLineService>();
            var paymentHistoryLineObj = GetService<ISaleOrderPaymentHistoryLineService>();

            var saleOrderPaymentDisplay = await SearchQuery(x => x.Id == id).Select(x => new SaleOrderPaymentDisplay
            {
                Date = x.Date,
                Amount = x.Amount,
                Note = x.Note,
                State = x.State
            }).FirstOrDefaultAsync();

            saleOrderPaymentDisplay.JournalLines = await paymentJournalLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Select(x => new SaleOrderPaymentJournalLineDisplay
            {
                Id = x.Id,
                JournalId = x.JournalId,
                Journal = new AccountJournalSimple
                {
                    Id = x.Journal.Id,
                    Name = x.Journal.Name
                },
                Amount = x.Amount

            }).ToListAsync();

            saleOrderPaymentDisplay.Lines = _mapper.Map<IEnumerable<SaleOrderPaymentHistoryLineDisplay>>(await paymentHistoryLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Include(x => x.SaleOrderLine).ToListAsync());


            return saleOrderPaymentDisplay;
        }


        public async Task<SaleOrderPayment> CreateSaleOrderPayment(SaleOrderPaymentSave val)
        {
            //Mapper
            var saleOrderPayment = _mapper.Map<SaleOrderPayment>(val);
            await CreateAsync(saleOrderPayment);

            return saleOrderPayment;
        }

        public async override Task<SaleOrderPayment> CreateAsync(SaleOrderPayment entity)
        {
            var sequenceServiceObj = GetService<IIRSequenceService>();
            entity.Name = await sequenceServiceObj.NextByCode("account.payment.customer.invoice");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertSaleOrderPaymentSequence();
                entity.Name = await sequenceServiceObj.NextByCode("account.payment.customer.invoice");
            }

            await base.CreateAsync(entity);

            return entity;
        }

        private async Task _InsertSaleOrderPaymentSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Payments customer invoices sequence",
                Code = "account.payment.customer.invoice",
                Prefix = "CUST.IN/{yyyy}/",
                Padding = 4
            });
        }

        public async Task ActionPayment(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            /// truy vấn đủ dữ liệu của saleorder payment
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move)
                //.Include(x => x.PaymentMove)
                .Include(x => x.Order).ThenInclude(s => s.OrderLines)
                .Include(x => x.Lines).ThenInclude(s => s.SaleOrderLine)
                .Include(x => x.JournalLines).ThenInclude(s => s.Journal).ThenInclude(s => s.DefaultDebitAccount)
                .ToListAsync();

            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///ghi sổ doang thu , công nợ lines    
                ///tạo hoa hồng cho bác sĩ , phụ tá , tư vấn
                var invoices = await _CreateInvoices(saleOrderPayment.Order, saleOrderPayment.Lines);
                await moveObj.ActionPost(invoices);
                ///update moveId của SaleOrderPayment
                saleOrderPayment.MoveId = invoices.FirstOrDefault().Id;

                var order = saleOrderPayment.Order;
                saleLineObj._GetInvoiceQty(order.OrderLines);
                saleLineObj._GetToInvoiceQty(order.OrderLines);
                saleLineObj._GetInvoiceAmount(order.OrderLines);
                saleLineObj._GetToInvoiceAmount(order.OrderLines);
                saleLineObj._ComputeInvoiceStatus(order.OrderLines);
                saleLineObj._ComputeLinePaymentRels(order.OrderLines);

                saleOrderObj._GetInvoiced(new List<SaleOrder>() { order });
                ///compute residual , paid của line trong saleOrder
                saleOrderObj._ComputeResidual(new List<SaleOrder>() { order });
                await saleOrderObj.UpdateAsync(new List<SaleOrder>() { order });

                ///vòng lặp phương thức thanh toán tạo move , move line
                var movePayments = await _PreparePaymentMoves(saleOrderPayment);

                var amlObj = GetService<IAccountMoveLineService>();
                foreach (var move in movePayments)
                    amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(movePayments);
                await moveObj.ActionPost(movePayments);

                /// update state = "posted"
                saleOrderPayment.State = "posted";

            }

            await UpdateAsync(saleOrderPayments);


        }

        public async Task<IEnumerable<AccountMove>> _CreateInvoices(SaleOrder order, IEnumerable<SaleOrderPaymentHistoryLine> self)
        {
            //param final: if True, refunds will be generated if necessary
            var saleLineObj = GetService<ISaleOrderLineService>();
            // Invoice values.
            var invoice_vals = await _PrepareInvoice(order);
            var linedict = await saleLineObj.SearchQuery(x => x.OrderId == order.Id).Include(x => x.ProductUOM).ToDictionaryAsync(x => x.Id, x => x);
            //Invoice line values (keep only necessary sections)
            foreach (var line in self)
            {

                if (linedict[line.SaleOrderLineId].QtyToInvoice == 0 && linedict[line.SaleOrderLineId].AmountToInvoice == 0)
                    continue;
                if ((linedict[line.SaleOrderLineId].QtyToInvoice > 0 && linedict[line.SaleOrderLineId].AmountToInvoice > 0) || (linedict[line.SaleOrderLineId].QtyToInvoice < 0) || (linedict[line.SaleOrderLineId].QtyToInvoice <= 0 && linedict[line.SaleOrderLineId].AmountToInvoice > 0))
                {
                    var moveline = await _PrepareInvoiceLineAsync(linedict[line.SaleOrderLineId], line.Amount);
                    invoice_vals.InvoiceLines.Add(moveline);
                }
            }
            var invoice_vals_list = new List<AccountMove>();


            if (!invoice_vals_list.Any())
                throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");



            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(new List<AccountMove>() { invoice_vals }, default_type: "out_invoice");

            return moves;
        }

        private async Task<AccountMove> _PrepareInvoice(SaleOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "out_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting sales journal for the company {CompanyId}.");

            var invoice_vals = new AccountMove
            {
                Ref = "",
                Type = "out_invoice",
                Narration = self.Note,
                InvoiceUserId = self.UserId,
                PartnerId = self.PartnerId,
                InvoiceOrigin = self.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = journal.CompanyId,
                InvoiceDate = self.DateOrder.Date,
                Date = self.DateOrder.Date
            };

            return invoice_vals;
        }

        private async Task<AccountMoveLine> _PrepareInvoiceLineAsync(SaleOrderLine self, decimal amount)
        {
            var commissionSettlementObj = GetService<ICommissionSettlementService>();
            var res = new AccountMoveLine
            {
                Name = self.Name,
                ProductId = self.ProductId,
                ProductUoMId = self.ProductUOMId,
                Quantity = self.QtyToInvoice == 0 ? 1 : self.QtyToInvoice,
                Discount = self.Discount,
                PriceUnit = amount,
                DiscountType = self.DiscountType,
                DiscountFixed = self.DiscountFixed,
                SalesmanId = self.SalesmanId
            };

            res.SaleLineRels.Add(new SaleOrderLineInvoice2Rel { OrderLine = self });

            var commissionSettlements = await commissionSettlementObj._PrepareCommission(res, self);
            if (commissionSettlements.Any())
            {
                foreach (var item in commissionSettlements)
                    res.CommissionSettlements.Add(item);
            }


            return res;
        }

        public async Task<IEnumerable<AccountMove>> _PreparePaymentMoves(SaleOrderPayment self)
        {
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();
            var all_move_vals = new List<AccountMove>();
            var account = await accountObj.GetAccountReceivableCurrentCompany();

            foreach (var payment in self.JournalLines)
            {
                decimal counterpart_amount = 0;
                AccountAccount liquidity_line_account = null;

                if (payment.Journal.Type == "advance")
                    counterpart_amount = -payment.Amount;
                else
                    counterpart_amount = payment.Amount;


                liquidity_line_account = payment.Journal.DefaultDebitAccount;


                var balance = counterpart_amount;
                var liquidity_amount = counterpart_amount;

                var rec_pay_line_name = "Khách hàng thanh toán";

                var liquidity_line_name = self.Name;

                var move_vals = new AccountMove
                {
                    Date = self.Date,
                    PartnerId = self.Order.PartnerId,
                    JournalId = payment.JournalId,
                    Journal = payment.Journal,
                    CompanyId = self.CompanyId,
                };

                var lines = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name = rec_pay_line_name,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        DateMaturity = self.Date,
                        AccountId = account.Id,
                        Account = account,
                        PaymentId = payment.Id,
                        PartnerId = self.Order.PartnerId,
                        Move = move_vals,
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        DateMaturity = self.Date,
                        AccountId = liquidity_line_account.Id,
                        Account = liquidity_line_account,
                        PaymentId = payment.Id,
                        PartnerId = self.Order.PartnerId,
                        Move = move_vals,
                    },
                };

                move_vals.Lines = lines;

                all_move_vals.Add(move_vals);
            }

            return all_move_vals;
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var saleOrderPayment = await SearchQuery(x => ids.Contains(x.Id) && x.State != "cancel")
                .Include(x => x.Move).ThenInclude(s => s.Lines)
                .Include(x => x.Order).ThenInclude(s => s.OrderLines)
                .Include(x => x.MoveLines).ThenInclude(s => s.Move).ThenInclude(c => c.Lines)
                .ToListAsync();


            foreach (var rec in saleOrderPayment)
            {
                ///check thanh toán trong mới cho hủy


                ///xử lý tìm các hóa đơn thanh toán để xóa
                foreach (var move in rec.MoveLines.Select(x => x.Move).Distinct().ToList())
                {
                    if (rec.MoveLines.Any())
                        await moveLineObj.RemoveMoveReconcile(move.Lines.Select(x => x.Id).ToList());

                    await moveObj.ButtonCancel(new List<Guid>() { move.Id });
                    await moveObj.Unlink(new List<Guid>() { move.Id });
                }

                /// tìm và xóa hóa đơn ghi sổ doanh thu , công nợ
                /// xóa các hoa hồng của bác sĩ 
                if(rec.Move.Lines.Any())
                    await moveLineObj.RemoveMoveReconcile(rec.Move.Lines.Select(x => x.Id).ToList());

                await moveObj.ButtonCancel(new List<Guid>() { rec.Move.Id });
                await moveObj.Unlink(new List<Guid>() { rec.Move.Id });


                //tính lại paid , residual saleorder line


                rec.State = "cancel";
            }

            await UpdateAsync(saleOrderPayment);


        }

        public override ISpecification<SaleOrderPayment> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.sale_order_payment_comp_rule":
                    return new InitialSpecification<SaleOrderPayment>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }
    }
}
