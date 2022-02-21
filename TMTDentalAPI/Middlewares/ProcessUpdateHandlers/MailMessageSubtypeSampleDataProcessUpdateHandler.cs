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
    public class MailMessageSubtypeSampleDataProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.1";
        private IServiceScopeFactory _serviceScopeFactory;
        public MailMessageSubtypeSampleDataProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    //Version version1 = new Version(_version);
                    //Version version2 = new Version(tenant.Version);
                    //if (version2.CompareTo(version1) >= 0)
                    //    return Task.CompletedTask;
                }

                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                if (!context.MailMessageSubtypes.Any())
                {
                    var Subtypes = new[]
                    {
                         new { Name =  "Phiếu điều trị" , IRModelName = "subtype_sale_order" },
                         new { Name =  "Dịch vụ" , IRModelName = "subtype_sale_order_line" },
                         new { Name =  "Thanh toán" , IRModelName = "subtype_sale_order_payment" },
                         new { Name =  "Lịch hẹn" , IRModelName = "subtype_appointment" },
                         new { Name =  "Tiếp nhận" , IRModelName = "subtype_receive" },
                         new { Name =  "Đợt khám" , IRModelName = "subtype_dotkham" },
                         new { Name =  "Ghi chú" , IRModelName = "subtype_comment" },
                    };

                    foreach (var type in Subtypes)
                    {
                        var mailMessageSubtype = new MailMessageSubtype { Name = type.Name };
                        context.MailMessageSubtypes.Add(mailMessageSubtype);
                        context.SaveChanges();

                        var iRmodelData = new IRModelData { Name = type.IRModelName, Module = "mail", ResId = mailMessageSubtype.Id.ToString(), Model = "mail.message.subtype" };
                        context.IRModelDatas.Add(iRmodelData);
                        context.SaveChanges();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
