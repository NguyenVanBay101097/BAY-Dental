using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailMessagesController : BaseApiController
    {
        private readonly IMailMessageService _mailMessageService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public MailMessagesController(IMailMessageService mailMessageService, IUnitOfWorkAsync unitOfWork)
        {
            _mailMessageService = mailMessageService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MessageFetch(MailMessageFetch val)
        {
            var res = await _mailMessageService.MessageFetch(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetLogsForPartner(LogForPartnerRequest val)
        {
            var res = await _mailMessageService.GetLogsForPartner(val);
            var actionLogs = res.GroupBy(x => x.Date.Value.Date).Select(x => new TimeLineLogForPartnerResponse
            {
                Date = x.Key.Date,
                Logs = x.Select(s => new LogForPartnerResponse
                {
                    Id = s.Id,
                    Date = s.Date.Value,
                    body = s.Body,
                    SubtypeName = s.Subtype.Name,
                    UserName = s.Author.Name
                }).OrderByDescending(s => s.Date).ToList()
            }).OrderByDescending(x => x.Date).ToList();

            return Ok(actionLogs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var actionLog = await _mailMessageService.GetByIdAsync(id);
            await _mailMessageService.DeleteAsync(actionLog);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}