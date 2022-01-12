using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ICommentService : IBaseService<MailMessage>
    {
        Task<MailMessage> CreateComment(string body = null, Guid? threadId = null, string threadModel = null, string messageType = "comment", string subtype = "subtype_comment");
    }
}
