using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class AddSalaryPaymentProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.4";
        private IServiceScopeFactory _serviceScopeFactory;

        public AddSalaryPaymentProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            // nếu version tenant nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                //if (tenant != null)
                //    return Task.CompletedTask;

                //Version version1 = new Version(_version);
                //Version version2 = new Version(tenant.Version);
                //if (version2.CompareTo(version1) >= 0)
                //    return Task.CompletedTask;

                var scopedService = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var salaryPayments = context.SalaryPayments.Where(x => x.State == "done" && x.Move != null).Include(x => x.Move).ThenInclude(x => x.Lines).ToList();
                var dict = new Dictionary<Guid, List<AccountMoveLine>>();

                foreach (var salary in salaryPayments)
                {
                    if (!dict.ContainsKey(salary.Id))
                    {
                        dict.Add(salary.Id, new List<AccountMoveLine>(salary.Move.Lines));
                    }
                }

                foreach (var item in dict)
                {
                    var lines = item.Value;
                    foreach (var line in lines)
                    {
                        line.SalaryPaymentId = item.Key;
                    }
                }
                context.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
