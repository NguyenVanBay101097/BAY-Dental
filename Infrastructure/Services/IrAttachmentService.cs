using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class IrAttachmentService : BaseService<IrAttachment>, IIrAttachmentService
    {
        public IrAttachmentService(IAsyncRepository<IrAttachment> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override async Task<IEnumerable<IrAttachment>> CreateAsync(IEnumerable<IrAttachment> entities)
        {
            foreach (var values in entities)
            {
                _CheckContents(values);
                _MakeThumbnail(values);
            }
            return await base.CreateAsync(entities);
        }

        private void _MakeThumbnail(IrAttachment values)
        {
        }

        private void _CheckContents(IrAttachment values)
        {
            if (string.IsNullOrEmpty(values.MineType))
                values.MineType = _ComputeMineType(values);
        }

        private string _ComputeMineType(IrAttachment values)
        {
            var provider = new FileExtensionContentTypeProvider();
            string mineType = null;
            if (!string.IsNullOrEmpty(values.MineType))
                mineType = values.MineType;
            if (mineType == null && !string.IsNullOrEmpty(values.DatasFname))
                provider.TryGetContentType(values.DatasFname, out mineType);
            if (mineType == null && !string.IsNullOrEmpty(values.Url))
                provider.TryGetContentType(values.Url, out mineType);
            //có thể viết hàm đoán minetype từ binary data, tạm thời viết sau
            return mineType ?? "application/octet-stream";
        }

        public Task<IrAttachment> GetAttachment(string resModel, Guid? resId, string resField)
        {
            return SearchQuery(x => x.ResModel == resModel && x.ResId == resId && x.ResField == resField).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<IrAttachment>> GetAttachments(string resModel, Guid? resId)
        {
            return await ListAsync(new InitialSpecification<IrAttachment>(x => x.ResModel == resModel && x.ResId == resId));
        }

        public async Task<IEnumerable<IrAttachmentBasic>> SearchRead(IrAttachmentSearchRead val)
        {
            ISpecification<IrAttachment> spec = new InitialSpecification<IrAttachment>(x => true);
            if (string.IsNullOrEmpty(val.Model))
                spec = spec.And(new InitialSpecification<IrAttachment>(x => x.ResModel == val.Model));
            if (val.ResId.HasValue)
                spec = spec.And(new InitialSpecification<IrAttachment>(x => x.ResId == val.ResId));
            var query = SearchQuery(spec.AsExpression());
            var res = await query.Select(x => new IrAttachmentBasic
            {
                Id = x.Id,
                Name = x.Name,
                DatasFname = x.DatasFname,
                MineType = x.MineType,
                Url = x.Url,
                UploadId = x.UploadId,
            }).ToListAsync();
            return res;
        }
    }
}
