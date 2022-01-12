using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IMailThreadMessageService
    {
        Task<MailMessage> MessagePost<T>(T record, string body, string subjectTypeId = "",
            string messageType = "notification") where T: BaseEntity;

      
    }
}
