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

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

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
            SaveLines(val, saleOrderPayment);
            await CreateAsync(saleOrderPayment);

            return saleOrderPayment;
        }

        private void SaveLines(SaleOrderPaymentSave val, SaleOrderPayment orderPayment)
        {
            var lineToRemoves = new List<SaleOrderPaymentHistoryLine>();

            foreach (var existLine in orderPayment.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                orderPayment.Lines.Remove(line);
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<SaleOrderPaymentHistoryLine>(line);
                    orderPayment.Lines.Add(item);
                }
                else
                {
                    var l = orderPayment.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                }
            }
        }

        //public async override Task<SaleOrderPayment> CreateAsync(SaleOrderPayment entity)
        //{
        //    var sequenceServiceObj = GetService<IIRSequenceService>();
        //    entity.Name = await sequenceServiceObj.NextByCode("account.payment.customer.invoice");
        //    if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
        //    {
        //        await _InsertSaleOrderPaymentSequence();
        //        entity.Name = await sequenceServiceObj.NextByCode("account.payment.customer.invoice");
        //    }

        //    await base.CreateAsync(entity);

        //    return entity;
        //}

        //private async Task _InsertSaleOrderPaymentSequence()
        //{
        //    var seqObj = GetService<IIRSequenceService>();
        //    await seqObj.CreateAsync(new IRSequence
        //    {
        //        Name = "Payments customer invoices sequence",
        //        Code = "account.payment.customer.invoice",
        //        Prefix = "CUST.IN/{yyyy}/",
        //        Padding = 4
        //    });
        //}

        public async Task ActionPayment(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var commissionSettlementObj = GetService<ICommissionSettlementService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentObj = GetService<IAccountPaymentService>();
            /// truy vấn đủ dữ liệu của saleorder payment
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move)
                .Include(x => x.Order).ThenInclude(s => s.OrderLines)
                .Include(x => x.Lines).ThenInclude(s => s.SaleOrderLine)
                .Include(x => x.JournalLines).ThenInclude(s => s.Journal).ThenInclude(s => s.DefaultDebitAccount)
                .ToListAsync();

            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///ghi sổ doang thu , công nợ lines               
                var invoice = await _CreateInvoices(saleOrderPayment);
                await moveObj.ActionPost(new List<AccountMove>() { invoice });

                ///tạo hoa hồng cho bác sĩ , phụ tá , tư vấn
                var commissions = await commissionSettlementObj._PrepareCommission(invoice);
                await commissionSettlementObj.CreateAsync(commissions);

                //tính lại paid , residual saleorder , lines
                await _ComputeSaleOrder(saleOrderPayment.OrderId);

                ///vòng lặp phương thức thanh toán tạo move , move line
                var payments = _PreparePayments(saleOrderPayment, invoice.Id);
                await paymentObj.CreateAsync(payments);
                await paymentObj.Post(payments.Select(x => x.Id).ToList());

                saleOrderPayment.MoveId = invoice.Id;
                foreach (var payment in payments)
                    saleOrderPayment.PaymentRels.Add(new SaleOrderPaymentAccountPaymentRel { PaymentId = payment.Id });
                /// update state = "posted"
                saleOrderPayment.State = "posted";
             
            }

            await UpdateAsync(saleOrderPayments);


        }

        public async Task<AccountMove> _CreateInvoices(SaleOrderPayment self)
        {
            //param final: if True, refunds will be generated if necessary
            var saleLineObj = GetService<ISaleOrderLineService>();
            // Invoice values.
            var invoice_vals = await _PrepareInvoice(self);
            foreach(var line in self.Lines)
            {
                if (line.Amount == 0)
                    continue;
                var moveline = _PrepareInvoiceLineAsync(line);
                invoice_vals.InvoiceLines.Add(moveline);
            }

            //var linedict = await saleLineObj.SearchQuery(x => x.OrderId == order.Id).Include(x => x.ProductUOM).ToDictionaryAsync(x => x.Id, x => x);
            ////Invoice line values (keep only necessary sections)
            //foreach (var line in self)
            //{

            //    if (linedict[line.SaleOrderLineId].QtyToInvoice == 0 && linedict[line.SaleOrderLineId].AmountToInvoice == 0)
            //        continue;
            //    if ((linedict[line.SaleOrderLineId].QtyToInvoice > 0 && linedict[line.SaleOrderLineId].AmountToInvoice > 0) || (linedict[line.SaleOrderLineId].QtyToInvoice < 0) || (linedict[line.SaleOrderLineId].QtyToInvoice <= 0 && linedict[line.SaleOrderLineId].AmountToInvoice > 0))
            //    {
            //        var moveline = await _PrepareInvoiceLineAsync(linedict[line.SaleOrderLineId], line.Amount);
            //        invoice_vals.InvoiceLines.Add(moveline);
            //    }
            //}
            //var invoice_vals_list = new List<AccountMove>();


            if (!invoice_vals.InvoiceLines.Any())
                throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");



            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(new List<AccountMove>() { invoice_vals }, default_type: "out_invoice");

            return invoice_vals;
        }

        private async Task<AccountMove> _PrepareInvoice(SaleOrderPayment self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "out_invoice", default_company_id: self.CompanyId);
            if (journal == null)
                throw new Exception($"Please define an accounting sales journal for the company {CompanyId}.");

            var invoice_vals = new AccountMove
            {
                Ref = "",
                Type = "out_invoice",
                Narration = self.Note,
                PartnerId = self.Order.PartnerId,
                InvoiceOrigin = self.Order.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = journal.CompanyId,
                Date = self.Date
            };

            return invoice_vals;
        }

        private AccountMoveLine _PrepareInvoiceLineAsync(SaleOrderPaymentHistoryLine self)
        {
            var res = new AccountMoveLine
            {
                Name = self.SaleOrderLine.Name,
                ProductId = self.SaleOrderLine.ProductId,
                ProductUoMId = self.SaleOrderLine.ProductUOMId,
                Quantity = 1,
                PriceUnit = self.Amount,
                //SalesmanId = self.SalesmanId
            };

            res.SaleLineRels.Add(new SaleOrderLineInvoice2Rel { OrderLineId = self.SaleOrderLineId });

           
            return res;
        }

        public IEnumerable<AccountPayment> _PreparePayments(SaleOrderPayment self, Guid moveId)
        {
            //var accountObj = GetService<IAccountAccountService>();
            //var accountJournalObj = GetService<IAccountJournalService>();
            //var all_move_vals = new List<AccountMove>();
            //var account = await accountObj.GetAccountReceivableCurrentCompany();

            //check journal payment > 0 mới xử lý ghi sổ
            var lines = self.JournalLines.Where(x => x.Amount > 0).ToList();
            var results = new List<AccountPayment>();
            foreach (var line in lines)
            {
                var payment = new AccountPayment
                {
                    JournalId = line.JournalId,
                    PartnerId = self.Order.PartnerId,
                    Amount = line.Amount,
                    PartnerType = "customer",
                    PaymentDate = self.Date,
                    PaymentType = "inbound",
                    CompanyId = line.Journal.CompanyId,
                };

                payment.AccountMovePaymentRels.Add(new AccountMovePaymentRel { MoveId = moveId });
                results.Add(payment);

                //decimal counterpart_amount = 0;
                //AccountAccount liquidity_line_account = null;

                //if (line.Journal.Type == "advance")
                //    counterpart_amount = -line.Amount;
                //else
                //    counterpart_amount = line.Amount;


                //liquidity_line_account = line.Journal.DefaultDebitAccount;


                //var balance = counterpart_amount;
                //var liquidity_amount = counterpart_amount;

                //var rec_pay_line_name = "Khách hàng thanh toán";

                //var liquidity_line_name = self.Name;

                //var move_vals = new AccountMove
                //{
                //    Date = self.Date,
                //    PartnerId = self.Order.PartnerId,
                //    JournalId = line.JournalId,
                //    Journal = line.Journal,
                //    CompanyId = self.CompanyId,
                //};

                //var lines = new List<AccountMoveLine>()
                //{
                //    new AccountMoveLine
                //    {
                //        Name = rec_pay_line_name,
                //        Debit = balance > 0 ? balance : 0,
                //        Credit = balance < 0 ? -balance : 0,
                //        DateMaturity = self.Date,
                //        AccountId = account.Id,
                //        Account = account,
                //        PaymentId = line.Id,
                //        PartnerId = self.Order.PartnerId,
                //        Move = move_vals,
                //    },
                //    new AccountMoveLine
                //    {
                //        Name = liquidity_line_name,
                //        Debit = balance < 0 ? -balance : 0,
                //        Credit = balance > 0 ? balance : 0,
                //        DateMaturity = self.Date,
                //        AccountId = liquidity_line_account.Id,
                //        Account = liquidity_line_account,
                //        PaymentId = line.Id,
                //        PartnerId = self.Order.PartnerId,
                //        Move = move_vals,
                //    },
                //};

                //move_vals.Lines = lines;

                //all_move_vals.Add(move_vals);
            }

            return results;
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentObj = GetService<IAccountPaymentService>();
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id) && x.State != "cancel")
                .Include(x => x.Move).ThenInclude(s => s.Lines)
                .Include(x => x.Order).ThenInclude(s => s.OrderLines)
                .Include(x=> x.PaymentRels).ThenInclude(s=>s.Payment)
                .ToListAsync();


            foreach (var res in saleOrderPayments)
            {
                ///check thanh toán trong mới cho hủy
                if (firstDayOfMonth < res.Date && res.Date > lastDayOfMonth)
                    throw new Exception("Bạn chỉ được hủy thanh toán trong tháng");

                ///xử lý tìm các payment update state = "cancel" va hóa đơn thanh toán để xóa
                var payment_ids = res.PaymentRels.Select(x => x.PaymentId).ToList();
                await paymentObj.CancelAsync(payment_ids);

                /// tìm và xóa hóa đơn ghi sổ doanh thu , công nợ
                /// xóa các hoa hồng của bác sĩ 
                if(res.Move.Lines.Any())
                    await moveLineObj.RemoveMoveReconcile(res.Move.Lines.Select(x => x.Id).ToList());

                await moveObj.ButtonCancel(new List<Guid>() { res.Move.Id });
                await moveObj.Unlink(new List<Guid>() { res.Move.Id });


                //tính lại paid , residual saleorder , lines
                await _ComputeSaleOrder(res.OrderId);


                res.State = "cancel";
            }

            await UpdateAsync(saleOrderPayments);


        }

        public async Task _ComputeSaleOrder(Guid saleOrderId)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderlineObj = GetService<ISaleOrderLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var order = await orderObj.SearchQuery(x => x.Id == saleOrderId).Include(x => x.OrderLines)              
                .FirstOrDefaultAsync();

            orderlineObj._GetInvoiceQty(order.OrderLines);
            orderlineObj._GetToInvoiceQty(order.OrderLines);
            orderlineObj._GetInvoiceAmount(order.OrderLines);
            orderlineObj._GetToInvoiceAmount(order.OrderLines);
            orderlineObj._ComputeInvoiceStatus(order.OrderLines);
            orderlineObj._ComputeLinePaymentRels(order.OrderLines);

            orderObj._GetInvoiced(new List<SaleOrder>() { order });
            ///compute residual , paid của line trong saleOrder
            orderObj._ComputeResidual(new List<SaleOrder>() { order });

            await orderlineObj.UpdateAsync(order.OrderLines);
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
