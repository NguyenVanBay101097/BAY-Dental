using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class MailMessageService : BaseService<MailMessage>, IMailMessageService
    {
        public MailMessageService(IAsyncRepository<MailMessage> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<MailMessageFormat>> MessageFetch(MailMessageFetch val)
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();

            ISpecification<MailMessage> spec = new InitialSpecification<MailMessage>(x => x.Notifications.Any(s => s.ResPartnerId == user.PartnerId));
            var messages = await SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated), limit: val.Limit)
            .Select(x => new MailMessageFormat
            {
                Id = x.Id,
                Body = x.Body,
                Date = x.Date,
                AuthorName = x.Author.Name,
                MessageType = x.MessageType,
                Subject = x.Subject,
                Model = x.Model,
                ResId = x.ResId,
            }).ToListAsync();
            return messages;
        }

        public async Task<MailMessage> GenerateMessageCreatedAppointment(Guid appointmentId)
        {
            var apObj = GetService<IAppointmentService>();
            var appointment = await apObj.SearchQuery(x => x.Id == appointmentId).Include(x => x.Partner).Include(x => x.User)
                .FirstOrDefaultAsync();

            var message = new MailMessage()
            {
                Subject = $"Lịch hẹn được tạo",
                Body = $"<span class=\"message-bold\">{appointment.User.Name}</span> vừa đặt lịch hẹn cho khách hàng <span class=\"message-bold\">{appointment.Partner.Name}</span> vào lúc <span class=\"message-bold\">{appointment.Date.ToString("HH:mm dd/MM/yyyy")}</span>",
                ResId = appointment.Id,
                RecordName = appointment.Name,
                Model = "appointment",
                AuthorId = appointment.PartnerId,
                MessageType = "notification",
                Author = appointment.Partner,
            };

            message.Recipients.Add(new MailMessageResPartnerRel { PartnerId = appointment.User.PartnerId });
            message.Notifications.Add(new MailNotification { ResPartnerId = appointment.User.PartnerId });

            return await CreateAsync(message);
        }

        public async Task<IEnumerable<MailMessageFormat>> MessageFormat(IEnumerable<Guid> ids)
        {
            var query = SearchQuery(x => ids.Contains(x.Id),
                orderBy: x => x.OrderByDescending(s => s.DateCreated));
            return await MessageFormatFromQuery(query);
        }

        public async Task<IEnumerable<MailMessageFormat>> MessageFormatFromQuery(IQueryable<MailMessage> query)
        {
            var messages = await query.Select(x => new MailMessageFormat
            {
                Id = x.Id,
                Body = x.Body,
                Date = x.Date,
                AuthorName = x.Author.Name,
                MessageType = x.MessageType,
                Subject = x.Subject,
                Model = x.Model,
                ResId = x.ResId,
            }).ToListAsync();

            var messageDict = messages.ToDictionary(x => x.Id, x => x);
            var messageIds = messageDict.Keys.ToArray();

            return messages;
        }
    }
}
