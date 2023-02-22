using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TMTDentalAPI.Hubs;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Services
{
    public class AppointmentHubService : IAppointmentHubService
    {
        private readonly IHubContext<AppointmentHub> _notificationHub;
        private readonly AppTenant _tenant;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppointmentHubService(IHubContext<AppointmentHub> notificationHub,
            ITenant<AppTenant> tenant,
            IHttpContextAccessor httpContextAccessor)
        {
            _notificationHub = notificationHub;
            _tenant = tenant?.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task BroadcastCreateAsync()
        {
            await CurrentTenantCompanyGroup.SendAsync("broadcastCreate");
        }

        public async Task BroadcastDeleteAsync()
        {
            await CurrentTenantCompanyGroup.SendAsync("broadcastDelete");
        }

        protected string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        private IClientProxy CurrentTenantCompanyGroup =>
            _notificationHub.Clients.Group(_tenant.Hostname.ToString() + "-" + CompanyId.ToString());
    }
}
