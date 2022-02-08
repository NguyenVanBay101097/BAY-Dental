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
    public class PartnerAddressProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.0";
        private IServiceScopeFactory _serviceScopeFactory;

        public PartnerAddressProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var partners = context.Partners.Where(x => x.Customer && (x.CityCode == "" || x.CityName == "" || x.DistrictCode == "" || x.DistrictName == "" || x.WardCode == "" || x.WardName == "")).ToList();
                foreach(var partner in partners)
                {
                    if (partner.CityCode == "")
                        partner.CityCode = null;
                    if (partner.CityName == "")
                        partner.CityName = null;
                    if (partner.DistrictCode == "")
                        partner.DistrictCode = null;
                    if (partner.DistrictName == "")
                        partner.DistrictName = null;
                    if (partner.WardCode == "")
                        partner.WardCode = null;
                    if (partner.WardName == "")
                        partner.WardName = null;
                }

                context.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
