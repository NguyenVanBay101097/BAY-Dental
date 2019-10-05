using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IIrAttachmentService
    {
        Task<IrAttachment> CreateAsync(IrAttachment attachment);
        Task<IrAttachment> GetByIdAsync(Guid id);
        Task<IEnumerable<IrAttachmentBasic>> SearchRead(IrAttachmentSearchRead val);
        Task DeleteAsync(IrAttachment attachment);
        Task<IrAttachment> GetAttachment(string resModel, Guid? resId, string resField);

        Task<IEnumerable<IrAttachment>> GetAttachments(string resModel, Guid? resId);

        Task DeleteAttachments(IEnumerable<IrAttachment> attachments);
    }
}
