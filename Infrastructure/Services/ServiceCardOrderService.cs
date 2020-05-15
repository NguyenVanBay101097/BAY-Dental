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
            foreach(var order in self)
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
            var saleLineObj = GetService<ISaleOrderLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include("OrderLines.CardType").Include("OrderLines.CardType.Product")
                .ToListAsync();

            foreach (var order in self)
            {
                order.State = "sale";
                foreach (var line in order.OrderLines)
                {
                    line.State = "sale";
                }
            }

            var invoices = await _CreateInvoices(self);
            var moveObj = GetService<IAccountMoveService>();
            await moveObj.ActionPost(invoices);

            var cards = await _CreateCards(self);
            var cardObj = GetService<IServiceCardCardService>();
            await cardObj.ButtonConfirm(cards);

            _ComputeResidual(self);
            await UpdateAsync(self);
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
            foreach(var order in self)
            {
                foreach(var line in order.OrderLines)
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
