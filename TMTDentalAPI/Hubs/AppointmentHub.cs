using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Hubs
{
    [Authorize]
    public class AppointmentHub : Hub
    {
        public async Task CreateUpdate(IEnumerable<AppointmentBasic> agr)
        {
            //luu

            //await Clients.All.SendAsync("ReceiveMessage", agr);
            var temp = CurrentTenantCompanyId();

            await Clients.Groups(CurrentTenantCompanyId()).SendAsync("Receive", agr);
        }

        public async Task Delete(IEnumerable<Guid> ids)
        {
            //await Clients.All.SendAsync("ReceiveMessage", agr);
            await Clients.Groups(CurrentTenantCompanyId()).SendAsync("ReceiveDelete", ids);
        }

        /// <summary>
        /// Called when an authenticated client connects, store in group for tenant
        /// </summary>
        /// <returns>A task</returns>
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, CurrentTenantCompanyId());

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Calls when an authenticated client disconnects
        /// </summary>
        /// <param name="exception">Any exception that occucrred</param>
        /// <returns>A task</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CurrentTenantCompanyId());
            await base.OnDisconnectedAsync(exception);

        }

        protected string CurrentTenantCompanyId()
        {
            var httpContext = Context.GetHttpContext();
            var host = httpContext.Request.Host.Host;
            var subDomain = string.Empty;
            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            subDomain = subDomain.Trim().ToLower();
            return subDomain + "-" + CompanyId.ToString();
        }

        protected Guid CompanyId
        {
            get
            {
                if (!Context.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = Context.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }
    }
}
