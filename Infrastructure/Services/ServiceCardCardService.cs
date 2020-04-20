using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ServiceCardCardService : BaseService<ServiceCardCard>, IServiceCardCardService
    {
        public ServiceCardCardService(IAsyncRepository<ServiceCardCard> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
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
    }
}
