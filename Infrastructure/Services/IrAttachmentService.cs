using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class IrAttachmentService : IIrAttachmentService
    {
        private readonly IIrAttachmentRepository _attachmentRepository;
        public IrAttachmentService(IIrAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public Task<IrAttachment> CreateAsync(IrAttachment attachment)
        {
            return _attachmentRepository.InsertAsync(attachment);
        }

        public async Task DeleteAsync(IrAttachment attachment)
        {
            await _attachmentRepository.DeleteAsync(attachment);
        }

        public async Task DeleteAttachments(IEnumerable<IrAttachment> attachments)
        {
            await _attachmentRepository.DeleteAsync(attachments);
        }

        public Task<IrAttachment> GetAttachment(string resModel, Guid? resId, string resField)
        {
            return _attachmentRepository.FirstOrDefaultAsync(new InitialSpecification<IrAttachment>(x => x.ResModel == resModel && x.ResId == resId && x.ResField == resField));
        }

        public async Task<IEnumerable<IrAttachment>> GetAttachments(string resModel, Guid? resId)
        {
            return await _attachmentRepository.ListAsync(new InitialSpecification<IrAttachment>(x => x.ResModel == resModel && x.ResId == resId));
        }

        public Task<IrAttachment> GetByIdAsync(Guid id)
        {
            return _attachmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<IrAttachmentBasic>> SearchRead(IrAttachmentSearchRead val)
        {
            ISpecification<IrAttachment> spec = new InitialSpecification<IrAttachment>(x => true);
            if (string.IsNullOrEmpty(val.Model))
                spec = spec.And(new InitialSpecification<IrAttachment>(x => x.ResModel == val.Model));
            if (val.ResId.HasValue)
                spec = spec.And(new InitialSpecification<IrAttachment>(x => x.ResId == val.ResId));
            var query = _attachmentRepository.SearchQuery(spec);
            var res = await query.Select(x => new IrAttachmentBasic
            {
                Id = x.Id,
                Name = x.Name,
                DatasFname = x.DatasFname,
                MineType = x.MineType
            }).ToListAsync();
            return res;
        }
    }
}
