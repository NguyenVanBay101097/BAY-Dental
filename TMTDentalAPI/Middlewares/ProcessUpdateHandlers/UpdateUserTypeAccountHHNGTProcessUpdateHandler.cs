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
    public class UpdateUserTypeAccountHHNGTProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.4";
        private IServiceScopeFactory _serviceScopeFactory;


        public UpdateUserTypeAccountHHNGTProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                //Version version1 = new Version(_version);
                //Version version2 = new Version(tenant.Version);
                //if (version2.CompareTo(version1) >= 0)
                //    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var modelData = context.IRModelDatas.Where(x => x.Name == "data_account_type_current_liabilities" && x.Module == "account").FirstOrDefault();
                if (modelData != null)
                {
                    var hhngtAccounts = context.AccountAccounts.Where(x => x.Code == "HHNGT").ToList();
                    foreach (var account in hhngtAccounts)
                        account.UserTypeId = Guid.Parse(modelData.ResId);
                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}


