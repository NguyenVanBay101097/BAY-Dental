using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IrAttachmentsController : ControllerBase
    {
        private readonly IIrAttachmentService _attachmentService;
        public IrAttachmentsController(IIrAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchRead(IrAttachmentSearchRead val)
        {
            var list = await _attachmentService.SearchRead(val);
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();
            await _attachmentService.DeleteAsync(attachment);
            return NoContent();
        }
    }
}