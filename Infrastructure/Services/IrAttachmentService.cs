using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public IrAttachmentService(IAsyncRepository<IrAttachment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
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
            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var res = await _mapper.ProjectTo<IrAttachmentBasic>(query).ToListAsync();
            return res;
        }

        public async Task UpdateListAttachmentRes(Guid resId, string resModel, IEnumerable<IrAttachment> atts)
        {
            var attObj = GetService<IIrAttachmentService>();
            var existAtts = await attObj.GetAttachments(resModel, resId);
            //remove 
            var toRemoves = new List<IrAttachment>();
            foreach (var ex in existAtts)
            {
                if (!atts.Any(x => x.Id == ex.Id))
                    toRemoves.Add(ex);
            }
            
            if(toRemoves.Any())
            await attObj.DeleteAsync(toRemoves);

            var toAdds = new List<IrAttachment>();
            foreach (var att in atts)
            {
                if (att.Id == Guid.Empty)
                {
                    var item = new IrAttachment()
                    {
                        ResModel = "dot.kham",
                        ResId = resId,
                        Name = att.Name,
                        Type = "url",
                        Url = att.Url,
                        CompanyId = CompanyId
                    };
                    toAdds.Add(item);
                }

            }
            if (toAdds.Any())
                await attObj.CreateAsync(toAdds);
        }

        //public override ISpecification<IrAttachment> RuleDomainGet(IRRule rule)
        //{
        //    var userObj = GetService<IUserService>();
        //    var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
        //    switch (rule.Code)
        //    {
        //        case "base.res_partner_rule":
        //            return new InitialSpecification<IrAttachment>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
        //        default:
        //            return null;
        //    }
        //}
    }
}
