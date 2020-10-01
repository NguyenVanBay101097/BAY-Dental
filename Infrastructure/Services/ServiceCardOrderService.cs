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
    public class ServiceCardOrderService : BaseService<ServiceCardOrder>, IServiceCardOrderService
    {
        private readonly IMapper _mapper;

        public ServiceCardOrderService(IAsyncRepository<ServiceCardOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ServiceCardOrderBasic>> GetPagedResultAsync(ServiceCardOrderPaged val)
        {
            ISpecification<ServiceCardOrder> spec = new InitialSpecification<ServiceCardOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ServiceCardOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<ServiceCardOrderBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ServiceCardOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (!states.Contains(order.State))
                    throw new Exception("Bạn chỉ có thể xóa đơn hàng ở trạng thái nháp hoặc hủy bỏ");
            }

            await DeleteAsync(self);
        }

        public override ISpecification<ServiceCardOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "service_card.service_card_order_comp_rule":
                    return new InitialSpecification<ServiceCardOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.Order")
                .Include("OrderLines.Cards")
                .Include("OrderLines.OrderLineInvoiceRels")
                .Include("OrderLines.OrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.OrderLineInvoiceRels.InvoiceLine.Move")
                .ToListAsync();

            var saleLineObj = GetService<ISaleOrderLineService>();
            var move_ids = new List<Guid>().AsEnumerable();
            var card_ids = new List<Guid>().AsEnumerable();
            foreach (var sale in self)
            {
                if (sale.AmountResidual < sale.AmountTotal)
                    throw new Exception("Không thể hủy đơn hàng đã có thanh toán");

                foreach (var line in sale.OrderLines)
                {
                    if (line.Cards.Any(x => x.State == "in_use"))
                        throw new Exception("Không thể hủy đơn hàng đã có thẻ tiền mặt đang sử dụng");

                    move_ids = move_ids.Union(line.OrderLineInvoiceRels.Select(x => x.InvoiceLine.Move.Id).Distinct().ToList());
                    card_ids = card_ids.Union(line.Cards.Select(x => x.Id)).ToList();
                }
            }

            await UpdateAsync(self);

            if (move_ids.Any())
            {
                var moveObj = GetService<IAccountMoveService>();
                await moveObj.ButtonDraft(move_ids);

                await moveObj.Unlink(move_ids);
            }

            if (card_ids.Any())
            {
                var cardObj = GetService<IServiceCardCardService>();
                await cardObj.Unlink(card_ids);
            }

            foreach (var sale in self)
            {
                foreach (var line in sale.OrderLines)
                    line.State = "draft";

                sale.State = "draft";
            }

            await UpdateAsync(self);
        }

        public async Task<ServiceCardOrder> CreateUI(ServiceCardOrderSave val)
        {
            var order = _mapper.Map<ServiceCardOrder>(val);
            if (string.IsNullOrEmpty(order.Name) || order.Name == "/")
            {
                var seqObj = GetService<IIRSequenceService>();
                order.Name = await seqObj.NextByCode("service.card.order");
                if (string.IsNullOrEmpty(order.Name))
                {
                    await _CreateSequence();
                    order.Name = await seqObj.NextByCode("service.card.order");
                }
            }

            _SaveOrderLines(val, order);

            var lineObj = GetService<IServiceCardOrderLineService>();
            lineObj.PrepareLines(order.OrderLines);

            _ComputeResidual(new List<ServiceCardOrder>() { order });
            _AmountAll(new List<ServiceCardOrder>() { order });

            return await CreateAsync(order);
        }

        public async Task<ServiceCardOrderDisplay> GetDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<ServiceCardOrderDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                throw new Exception("Not found");

            var saleLineObj = GetService<IServiceCardOrderLineService>();
            res.OrderLines = await _mapper.ProjectTo<ServiceCardOrderLineDisplay>(saleLineObj.SearchQuery(x => x.OrderId == id, orderBy: x => x.OrderBy(s => s.Sequence))).ToListAsync();

            res.CardCount = res.OrderLines.Sum(x => x.CardCount);
            return res;
        }

        private void _AmountAll(IEnumerable<ServiceCardOrder> self)
        {
            foreach (var order in self)
            {
                var totalAmountUntaxed = 0M;

                foreach (var line in order.OrderLines)
                {
                    totalAmountUntaxed += line.PriceSubTotal;
                }

                order.AmountTotal = Math.Round(totalAmountUntaxed);
            }
        }

        public void _ComputeResidual(IEnumerable<ServiceCardOrder> self)
        {
            foreach (var order in self)
            {
                var invoices = order.OrderLines.SelectMany(x => x.OrderLineInvoiceRels)
                    .Select(x => x.InvoiceLine).Select(x => x.Move).Distinct().ToList();
                decimal? residual = 0M;
                foreach (var invoice in invoices)
                {
                    if (invoice.Type != "out_invoice" && invoice.Type != "out_refund")
                        continue;
                    if (invoice.Type == "out_invoice")
                        residual += invoice.AmountResidual;
                    else
                        residual -= invoice.AmountResidual;
                }

                order.AmountResidual = residual;
            }
        }


        private void _SaveOrderLines(ServiceCardOrderSave val, ServiceCardOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<ServiceCardOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
                order.OrderLines.Remove(line);

            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<ServiceCardOrderLine>(line);
                    saleLine.Sequence = sequence++;
                    saleLine.Order = order;
                    order.OrderLines.Add(saleLine);
                }
                else
                {
                    var saleLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (saleLine != null)
                    {
                        _mapper.Map(line, saleLine);
                        saleLine.Sequence = sequence++;
                    }
                }
            }
        }

        public async Task UpdateUI(Guid id, ServiceCardOrderSave val)
        {
            var order = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines).FirstOrDefaultAsync();
            order = _mapper.Map(val, order);

            _SaveOrderLines(val, order);

            var lineObj = GetService<IServiceCardOrderLineService>();
            lineObj.PrepareLines(order.OrderLines);

            _ComputeResidual(new List<ServiceCardOrder>() { order });
            _AmountAll(new List<ServiceCardOrder>() { order });

            await UpdateAsync(order);
        }

        private async Task _CreateSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "service.card.order",
                Name = "Service Card Order Sequence",
                Prefix = "CO",
                Padding = 5
            });
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include(x => x.Payments)
                .ToListAsync();

            await _CreateAccountMove(self);
        }

        public async Task CreateAndPaymentServiceCard(CreateAndPaymentServiceCardOrderVm val)
        {
            ///tạo mới ServiceCardOrder
            var order = new ServiceCardOrder();
            if (string.IsNullOrEmpty(order.Name) || order.Name == "/")
            {
                var seqObj = GetService<IIRSequenceService>();
                order.Name = await seqObj.NextByCode("service.card.order");
                if (string.IsNullOrEmpty(order.Name))
                {
                    await _CreateSequence();
                    order.Name = await seqObj.NextByCode("service.card.order");
                }
            }

            order.PartnerId = val.PartnerId;
            order.UserId = val.UserId;
            order.CompanyId = val.CompanyId;
            order.AmountRefund = val.AmountRefund;
            /// xử lý thêm orderlines vào servicecardorder
            if (!val.OrderLines.Any())
                throw new Exception("không tìm thấy danh sách thẻ trong đơn bán thẻ");

            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<ServiceCardOrderLine>(line);
                    saleLine.Sequence = sequence++;
                    saleLine.Order = order;
                    order.OrderLines.Add(saleLine);
                }             
            }

            ///xử lý thêm payments  vào servicecardorder
            if (!val.Payments.Any())
                throw new Exception("không tìm thấy thanh toán nào trong đơn bán thẻ");

            foreach (var pay in val.Payments)
            {
                if (pay.Id == Guid.Empty)
                {
                    var payOrder = new ServiceCardOrderPayment();
                    payOrder.Amount = pay.Amount;
                    payOrder.JournalId = pay.JournalId;
                    payOrder.Order = order;
                    order.Payments.Add(payOrder);
                }              
            }


            var lineObj = GetService<IServiceCardOrderLineService>();
            lineObj.PrepareLines(order.OrderLines);

            _ComputeResidual(new List<ServiceCardOrder>() { order });
            _AmountAll(new List<ServiceCardOrder>() { order });

            await CreateAsync(order);

            // xử lý thanh toán

            await _CreateAccountMove(new List<ServiceCardOrder>() { order });

        }

        public async Task _CreateAccountMove(IEnumerable<ServiceCardOrder> self)
        {
            var accountObj = GetService<IAccountAccountService>();
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var journalObj = GetService<IAccountJournalService>();
            var cardTypeObj = GetService<IServiceCardTypeService>();

            var income_account = await accountObj.GetAccountIncomeCurrentCompany();
            if (income_account == null)
                throw new Exception("Không tìm thấy tài khoản doanh thu mặc định");

            var order_account = await accountObj.GetAccountReceivableCurrentCompany();
            if (order_account == null)
                throw new Exception("Không tìm thấy tài khoản receivable mặc định");

            var journal = await journalObj.SearchQuery(x => x.Type == "sale" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (journal == null)
                throw new Exception("Vui lòng tạo nhật ký bán hàng cho công ty này.");

            foreach (var order in self.Where(x => !x.AccountMoveId.HasValue))
            {
                var sub_total = order.OrderLines.Sum(x => x.PriceSubTotal);
                if (sub_total == 0)
                    continue;

                var move = new AccountMove
                {
                    Ref = order.Name,
                    Journal = journal,
                    JournalId = journal.Id,
                    Date = order.DateOrder,
                    PartnerId = order.PartnerId,
                    CompanyId = journal.CompanyId,
                };

                await moveObj.CreateAsync(move);

                var amls = new List<AccountMoveLine>();
                var card_type_ids = order.OrderLines.Select(x => x.CardTypeId).Distinct().ToList();
                var card_types = await cardTypeObj.SearchQuery(x => card_type_ids.Contains(x.Id)).Include(x => x.Product).ToListAsync();
                var card_type_dict = card_types.ToDictionary(x => x.Id, x => x);
                foreach (var line in order.OrderLines)
                {
                    var amount = line.PriceSubTotal; //price sub total phải có cả chiết khấu tổng
                    var cardType = card_type_dict[line.CardTypeId];
                    //Create a move for the line for the order line
                    amls.Add(new AccountMoveLine
                    {
                        Name = cardType.Name,
                        Quantity = line.ProductUOMQty,
                        ProductId = cardType.ProductId,
                        ProductUoMId = cardType.Product.UOMId,
                        AccountId = income_account.Id,
                        Account = income_account,
                        Credit = amount > 0 ? amount : 0,
                        Debit = amount < 0 ? -amount : 0,
                        PartnerId = order.PartnerId,
                        Move = move,
                    });
                }

                var journal_ids = order.Payments.Select(x => x.JournalId).Distinct().ToList();
                var journals = await journalObj.SearchQuery(x => journal_ids.Contains(x.Id)).Include(x => x.DefaultCreditAccount)
                    .Include(x => x.DefaultDebitAccount).ToListAsync();
                var journal_dict = journals.ToDictionary(x => x.Id, x => x);

                foreach (var payment in order.Payments)
                {
                    var amount = payment.Amount;
                    var paymentJournal = journal_dict[payment.JournalId];
                    amls.Add(new AccountMoveLine
                    {
                        AccountId = amount > 0 ? paymentJournal.DefaultDebitAccount.Id : paymentJournal.DefaultCreditAccount.Id,
                        Account = amount > 0 ? paymentJournal.DefaultDebitAccount : paymentJournal.DefaultCreditAccount,
                        Credit = amount < 0 ? -amount : 0,
                        Debit = amount > 0 ? amount : 0,
                        PartnerId = order.PartnerId,
                        Move = move,
                    });
                }

                await amlObj.CreateAsync(amls);

                await moveObj.Write(new List<AccountMove>() { move });
                await moveObj.ActionPost(new List<AccountMove>() { move });

                order.State = "done";
                order.AccountMove = move;
                await UpdateAsync(order);
            }
        }

        public async Task<AccountMove> _CreateAccountMove(ServiceCardOrder self, DateTime dt, string mref, AccountJournal journal)
        {
            var moveObj = GetService<IAccountMoveService>();
            return await moveObj.CreateAsync(new AccountMove
            {
                Ref = mref,
                Journal = journal,
                JournalId = journal.Id,
                Date = dt,
                InvoiceUserId = self.UserId,
                PartnerId = self.PartnerId,
                CompanyId = journal.CompanyId,
            });
        }

        private object _PrepareLine(ServiceCardOrderLine orderLine)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateResidual(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.OrderLines)
                .Include("OrderLines.OrderLineInvoiceRels").Include("OrderLines.OrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.OrderLineInvoiceRels.InvoiceLine.Move").ToListAsync();

            _ComputeResidual(self);
            await UpdateAsync(self);
        }

        public async Task<IEnumerable<AccountMove>> _CreateInvoices(IEnumerable<ServiceCardOrder> self, bool final = false)
        {
            //param final: if True, refunds will be generated if necessary

            var saleLineObj = GetService<IServiceCardOrderLineService>();
            var invoice_vals_list = new List<AccountMove>();
            foreach (var order in self)
            {
                // Invoice values.
                var invoice_vals = await _PrepareInvoice(order);

                //Invoice line values (keep only necessary sections)
                foreach (var line in order.OrderLines)
                {
                    invoice_vals.InvoiceLines.Add(saleLineObj.PrepareInvoiceLine(line));
                }

                if (!invoice_vals.InvoiceLines.Any())
                    throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");

                invoice_vals_list.Add(invoice_vals);
            }

            if (!invoice_vals_list.Any())
                throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");

            //3) Manage 'final' parameter: transform out_invoice to out_refund if negative.
            var out_invoice_vals_list = new List<AccountMove>();
            var refund_invoice_vals_list = new List<AccountMove>();
            if (final)
            {
                foreach (var invoice_vals in invoice_vals_list)
                {
                    if (invoice_vals.InvoiceLines.Sum(x => x.Quantity * x.PriceUnit) < 0)
                    {
                        foreach (var l in invoice_vals.InvoiceLines)
                            l.Quantity = -l.Quantity;
                        invoice_vals.Type = "out_refund";
                        refund_invoice_vals_list.Add(invoice_vals);
                    }
                    else
                        out_invoice_vals_list.Add(invoice_vals);
                }
            }
            else
                out_invoice_vals_list = invoice_vals_list;

            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(out_invoice_vals_list, default_type: "out_invoice");
            moves = moves.Concat(await moveObj.CreateMoves(refund_invoice_vals_list, default_type: "out_refund"));

            return moves;
        }

        private async Task<IEnumerable<ServiceCardCard>> _CreateCards(IEnumerable<ServiceCardOrder> self)
        {
            var cardObj = GetService<IServiceCardCardService>();
            var saleLineObj = GetService<IServiceCardOrderLineService>();

            var card_vals_list = new List<ServiceCardCard>();
            foreach (var order in self)
            {
                foreach (var line in order.OrderLines)
                {
                    for (var i = 0; i < line.ProductUOMQty; i++)
                    {
                        var card_vals = saleLineObj.PrepareCard(line);
                        card_vals_list.Add(card_vals);
                    }
                }
            }

            await cardObj.CreateAsync(card_vals_list);
            return card_vals_list;
        }

        private async Task<AccountMove> _PrepareInvoice(ServiceCardOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "out_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting sales journal for the company {CompanyId}.");

            var invoice_vals = new AccountMove
            {
                Ref = "",
                Type = "out_invoice",
                InvoiceUserId = self.UserId,
                PartnerId = self.PartnerId,
                InvoiceOrigin = self.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = journal.CompanyId,
            };

            return invoice_vals;
        }
    }
}
