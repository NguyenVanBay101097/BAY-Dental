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

        public async Task<ServiceCardOrder> CreateUI(ServiceCardOrderSave val)
        {
            var order = _mapper.Map<ServiceCardOrder>(val);
            order.CompanyId = CompanyId;

            var seqObj = GetService<IIRSequenceService>();
            order.Name = await seqObj.NextByCode("service.card.order");
            if (string.IsNullOrEmpty(order.Name))
            {
                await _CreateSequence();
                order.Name = await seqObj.NextByCode("service.card.order");
            }

            return await CreateAsync(order);
        }

        public async Task UpdateUI(Guid id, ServiceCardOrderSave val)
        {
            var order = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            order = _mapper.Map(val, order);

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
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var order in self)
                order.State = "sale";
            await UpdateAsync(self);

            var cards = _CreateCards(self);
            var cardObj = GetService<IServiceCardCardService>();
            await cardObj.ActionActive(cards);

            var moves = await _CreateInvoices(self);
            var moveObj = GetService<IAccountMoveService>();
            await moveObj.ActionPost(moves);
        }

        private IEnumerable<ServiceCardCard> _CreateCards(IEnumerable<ServiceCardOrder> self)
        {
            var card_vals_list = new List<ServiceCardCard>();
            foreach(var order in self)
            {
                if (order.BuyType == "one")
                {
                    var partner_id = order.InheritedPartnerId.HasValue ? order.InheritedPartnerId.Value : order.PartnerId;
                    var card_vals = _PrepareCard(order, partner_id: partner_id);
                    card_vals_list.Add(card_vals);
                } 
                else if (order.BuyType == "many")
                {
                    foreach(var rel in order.PartnerRels)
                    {
                        var card_vals = _PrepareCard(order, partner_id: rel.PartnerId);
                        card_vals_list.Add(card_vals);
                    }
                }
            }

            return card_vals_list;
        }

        private ServiceCardCard _PrepareCard(ServiceCardOrder self, Guid? partner_id = null)
        {
            return new ServiceCardCard
            {
                CardTypeId = self.CardTypeId,
                PartnerId = partner_id,
                ActivatedDate = self.ActivatedDate,
                Amount = self.CardType.Amount,
                Residual = self.CardType.Amount,
            };
        }

        private async Task<IEnumerable<AccountMove>> _CreateInvoices(IEnumerable<ServiceCardOrder> self)
        {
            var moveObj = GetService<IAccountMoveService>();
            var invoice_vals_list = new List<AccountMove>();
            foreach (var order in self)
            {
                // Invoice values.
                var invoice_vals = await _PrepareInvoice(order);

                invoice_vals.InvoiceLines.Add(_PrepareInvoiceLine(order));
               
                await moveObj.CreateMoves(new List<AccountMove>() { invoice_vals }, default_type: "out_invoice");

                order.MoveId = invoice_vals.Id;
                invoice_vals_list.Add(invoice_vals);
            }

            await UpdateAsync(self);

            return invoice_vals_list;
        }

        private AccountMoveLine _PrepareInvoiceLine(ServiceCardOrder self)
        {
            var res = new AccountMoveLine
            {
                Name = self.CardType.Name,
                ProductId = self.CardType.ProductId,
                ProductUoMId = self.CardType.Product.UOMId,
                Quantity = self.Cards.Count,
                PriceUnit = self.CardType.Price,
            };

            return res;
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
