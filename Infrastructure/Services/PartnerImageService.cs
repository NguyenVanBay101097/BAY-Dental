using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMTDentalAPI.ViewModels;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerImageService : BaseService<PartnerImage>, IPartnerImageService
    {
        private readonly IMapper _mapper;
        public PartnerImageService(IAsyncRepository<PartnerImage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<PartnerImageBasic>> SearchRead(PartnerImageSearchRead val)
        {
            var query = SearchQuery();
            if (val.DotKhamId.HasValue)
                query = query.Where(x => x.DotkhamId == val.DotKhamId);
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var list = await query.OrderByDescending(x => x.Date).Select(x => new PartnerImageBasic
            {
                Date = x.Date,
                Id = x.Id,
                Name = x.Name,
                DotKhamId = x.DotkhamId,
                Note = x.Note,
                PartnerId = x.PartnerId,
                UploadId = x.UploadId
            }).ToListAsync();
            return list;
        }

        public async Task<IEnumerable<PartnerImageBasic>> BinaryUploadPartnerImage(UploadPartnerImageViewModel val)
        {
            var list = new List<PartnerImage>();
            var tasks = new List<Task<UploadResult>>();
            foreach (var file in val.files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    tasks.Add(_HandleUploadAsync(Convert.ToBase64String(memoryStream.ToArray()), file.FileName));
                }
            }

            var result = await Task.WhenAll(tasks);

            foreach (var item in result)
            {
                if (item == null)
                    throw new Exception("Hình không hợp lệ hoặc vượt quá kích thước cho phép.");
                var partnerImage = new PartnerImage()
                {
                    PartnerId = val.PartnerId,
                    DotkhamId = val.DotkhamId,
                    Date = val.Date ?? DateTime.Now,
                    Name = item.FileName,
                    Note = val.Note,
                    UploadId = item.FileUrl,
                };

                list.Add(partnerImage);
            }

            await CreateAsync(list);

            var res = _mapper.Map<IEnumerable<PartnerImageBasic>>(list);
            return res;
        }

        private Task<UploadResult> _HandleUploadAsync(string base64, string fileName)
        {
            var uploadObj = GetService<IUploadService>();
            return uploadObj.UploadBinaryAsync(base64, fileName: fileName);
        }
    }
}
