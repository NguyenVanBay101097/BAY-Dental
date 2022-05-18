using ApplicationCore.Entities;
using ApplicationCore.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
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
        private readonly AppTenant _tenant;
        private readonly ICurrentUser _currentUser;

        public AppointmentHub(IOptions<AppTenant> tenant,
            ICurrentUser currentUser)
        {
            _tenant = tenant?.Value;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Called when an authenticated client connects, store in group for tenant
        /// </summary>
        /// <returns>A task</returns>
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName());
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Calls when an authenticated client disconnects
        /// </summary>
        /// <param name="exception">Any exception that occucrred</param>
        /// <returns>A task</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName());
            await base.OnDisconnectedAsync(exception);

        }

        protected string GetGroupName()
        {
            return $"{(_tenant?.Id.ToString() ?? "localhost")}-{_currentUser.CompanyId?.ToString()}";
        }
    }
}
