using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CommentService : BaseService<MailMessage>, ICommentService
    {
        private readonly IMapper _mapper;
        private readonly IIRModelDataService _iRModelDataService;
        public CommentService(IAsyncRepository<MailMessage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IIRModelDataService iRModelDataService)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _iRModelDataService = iRModelDataService;
        }

        public async Task<MailMessage> CreateComment(string body = null, Guid? threadId = null, string threadModel = null, string messageType = "comment", string subtype = "subtype_comment")
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();
            var refSubtype = await _iRModelDataService.GetRef<MailMessageSubtype>($"mail.{subtype}");
            return await CreateAsync(new MailMessage
            {
                Body = body,
                ResId = threadId,
                Model = threadModel,
                MessageType = messageType,
                SubtypeId = refSubtype?.Id,
                AuthorId = user?.PartnerId
            });
        }

    }
}
