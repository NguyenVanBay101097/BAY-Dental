using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IMailThreadMessageService
    {
        Task<MailMessage> MessagePost<T>(T record, string body, DateTime? date = null, string subjectTypeId = "",
            string messageType = "notification") where T : BaseEntity;

        Task<MailMessage> MessagePost(string model,
              Guid? resId,
              string body,
              DateTime? date = null,
              string subjectTypeId = "mail.subtype_comment",
              string messageType = "notification");
    }
}
