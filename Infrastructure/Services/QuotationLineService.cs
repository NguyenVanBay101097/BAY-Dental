using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class QuotationLineService : BaseService<QuotationLine>, IQuotationLineService
    {
        private readonly IMapper _mapper;

        public QuotationLineService(IMapper mapper, IAsyncRepository<QuotationLine> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<GetQuotationLineDisplayDetail> GetQuotationLineDisplayDetail(Guid id)
        {
            var line = await SearchQuery(x => x.Id == id)
                .Include("QuotationLineToothRels.Tooth")
                .Include(x => x.ToothCategory)
                .Include(x => x.AdvisoryUser)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetQuotationLineDisplayDetail>(line);
        }
    }
}
