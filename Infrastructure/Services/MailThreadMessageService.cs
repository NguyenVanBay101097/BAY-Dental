using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MailThreadMessageService : IMailThreadMessageService
    {
        private IAsyncRepository<MailMessage> _mailMessageRepository;
        private ICurrentUser _currentUser;
        private IIRModelDataService _modelDataService;
        private IUserService _userService;

        public MailThreadMessageService(IAsyncRepository<MailMessage> mailMessageRepository,
            ICurrentUser currentUser,
            IIRModelDataService modelDataService,
            IUserService userService)
        {
            _mailMessageRepository = mailMessageRepository;
            _currentUser = currentUser;
            _modelDataService = modelDataService;
            _userService = userService;
        }

        public async Task<MailMessage> MessagePost<T>(T record, string body, DateTime? date,string subjectTypeId = "mail.subtype_comment",
            string messageType = "notification") where T : BaseEntity
        {
            MailMessageSubtype subtype = await _modelDataService.GetRef<MailMessageSubtype>(subjectTypeId);
            var user = await _userService.GetByIdAsync(_currentUser.Id);
            var msg = new MailMessage()
            {
                MessageType = messageType,
                Model = typeof(T).Name,
                ResId = record.Id,
                Body = body,
                SubtypeId = subtype.Id,
                AuthorId = user.PartnerId,
                Date = date ?? DateTime.Now /// if date exist get value : default datetime.Now
            };

            await _mailMessageRepository.InsertAsync(msg);
            return msg;
        }

        public async Task<MailMessage> MessagePost(string model,
            Guid? resId,
            string body,
            DateTime? date,
            string subjectTypeId = "mail.subtype_comment",
            string messageType = "notification")
        {
            MailMessageSubtype subtype = await _modelDataService.GetRef<MailMessageSubtype>(subjectTypeId);
            var user = await _userService.GetByIdAsync(_currentUser.Id);
            var msg = new MailMessage()
            {
                MessageType = messageType,
                Model = model,
                ResId = resId,
                Body = body,
                SubtypeId = subtype.Id,
                AuthorId = user.PartnerId,
                Date = date ?? DateTime.Now /// if date exist get value : default datetime.Now
            };

            await _mailMessageRepository.InsertAsync(msg);
            return msg;
        }
    }
}
