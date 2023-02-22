using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IIrAttachmentService: IBaseService<IrAttachment>
    {
        Task<IEnumerable<IrAttachmentBasic>> SearchRead(IrAttachmentSearchRead val);
        Task<IrAttachment> GetAttachment(string resModel, Guid? resId, string resField);

        Task<IEnumerable<IrAttachment>> GetAttachments(string resModel, Guid? resId);
        Task UpdateListAttachmentRes(Guid resId, string resModel, IEnumerable<IrAttachment> atts);

    }
}
