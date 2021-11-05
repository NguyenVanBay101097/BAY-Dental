using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class CardCardAndCardTypeProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.0";
        private IServiceScopeFactory _serviceScopeFactory;


        public CardCardAndCardTypeProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                //loại thẻ thành viên
                var cardTypeRule = context.IRRules.Where(x => x.Code == "member_card.card_type_comp_rule").FirstOrDefault();
                if (cardTypeRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "CardType").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Loại thẻ thành viên", Model = "CardType" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "member_card.card_type_comp_rule",
                        Name = "Card Type company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                //tạo rule thẻ thành viên
                var cardCardRule = context.IRRules.Where(x => x.Code == "member_card.card_card_comp_rule").FirstOrDefault();
                if (cardTypeRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "CardCard").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Thẻ thành viên", Model = "CardCard" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "member_card.card_card_comp_rule",
                        Name = "Card Card company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
