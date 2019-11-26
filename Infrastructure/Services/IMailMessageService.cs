using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IMailMessageService: IBaseService<MailMessage>
    {
        Task<MailMessage> GenerateMessageCreatedAppointment(Guid appointmentId);
        Task<IEnumerable<MailMessageFormat>> MessageFetch(MailMessageFetch val);
        Task<IEnumerable<MailMessageFormat>> MessageFormat(IEnumerable<Guid> ids);
    }
}
