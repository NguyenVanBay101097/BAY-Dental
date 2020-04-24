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
    public class ServiceCardCardService : BaseService<ServiceCardCard>, IServiceCardCardService
    {
        private readonly IMapper _mapper;

        public ServiceCardCardService(IAsyncRepository<ServiceCardCard> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task ActionActive(IEnumerable<ServiceCardCard> self)
        {
            var cardTypeObj = GetService<IServiceCardTypeService>();
            foreach(var card in self)
            {
                if (!card.ActivatedDate.HasValue)
                    card.ActivatedDate = DateTime.Today;
                var active_date = card.ActivatedDate.Value;
                var expire_date = cardTypeObj.GetPeriodEndDate(card.CardType, active_date);

                card.State = "in_use";
                card.ExpiredDate = expire_date;
            }

            await UpdateAsync(self);
        }

        public override async Task<IEnumerable<ServiceCardCard>> CreateAsync(IEnumerable<ServiceCardCard> self)
        {
            var seqObj = GetService<IIRSequenceService>();
            foreach (var card in self)
            {
                if (string.IsNullOrEmpty(card.Name) || card.Name == "/")
                {
                    card.Name = await seqObj.NextByCode("sequence_seq_service_card_nb");
                    if (string.IsNullOrEmpty(card.Name))
                    {
                        await CreateCardSequence();
                        card.Name = await seqObj.NextByCode("sequence_seq_service_card_nb");
                    }
                }
            }

            await base.CreateAsync(self);
            return self;
        }

        private async Task CreateCardSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Prefix = "SC",
                Padding = 6,
                NumberIncrement = 1,
                NumberNext = 1,
                Code = "sequence_seq_service_card_nb",
                Name = "Service Card Sequence"
            });
        }

        public async Task<PagedResult2<ServiceCardCardBasic>> GetPagedResultAsync(ServiceCardCardPaged val)
        {
            ISpecification<ServiceCardCard> spec = new InitialSpecification<ServiceCardCard>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<ServiceCardCardBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ServiceCardCardBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public void _ComputeResidual(IEnumerable<ServiceCardCard> self)
        {
            foreach(var card in self)
            {
                var total_apply_sale = card.SaleOrderCardRels.Sum(x => x.Amount);
                card.Residual = card.Amount - total_apply_sale;
            }
        }

        public async Task<IEnumerable<ServiceCardCard>> _ComputeResidual(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.SaleOrderCardRels).ToListAsync();
            _ComputeResidual(self);
            await UpdateAsync(self);

            return self;
        }
    }
}
