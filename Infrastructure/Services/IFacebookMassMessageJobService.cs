using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IFacebookMassMessageJobService
    {
        Task SendMessage(string db, Guid massMessagingId);
    }
}
