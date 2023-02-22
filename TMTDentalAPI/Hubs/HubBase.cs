using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TMTDentalAPI.Hubs
{
    [Authorize]
    public class HubBase : Hub
    {
        /// <summary>
        /// Called when an authenticated client connects, store in group for tenant
        /// </summary>
        /// <returns>A task</returns>
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, CurrentTenantId());
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Calls when an authenticated client disconnects
        /// </summary>
        /// <param name="exception">Any exception that occucrred</param>
        /// <returns>A task</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CurrentTenantId());
            await base.OnDisconnectedAsync(exception);
        }

        protected string CurrentTenantId()
        {
            var httpContext = Context.GetHttpContext();
            var host = httpContext.Request.Host.Host;
            var subDomain = string.Empty;
            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            subDomain = subDomain.Trim().ToLower();
            return subDomain;
        }
    }

}
