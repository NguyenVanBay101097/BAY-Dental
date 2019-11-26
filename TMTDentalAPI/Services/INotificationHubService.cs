using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Services
{
    public interface INotificationHubService
    {
        Task BroadcastNotificationAsync(MailMessageFormat message);
    }
}
