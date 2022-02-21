using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseApiController
    {
        private ICommentService _commentService;
        private IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CommentsController(ICommentService commentService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _commentService = commentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentRequest val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _commentService.CreateComment(body: val.Body, threadId: val.ThreadId, threadModel: val.ThreadModel);       
            _unitOfWork.Commit();

            var basic = _mapper.Map<MailMessageBasic>(res);
            return Ok(basic);
        }

    }
}
