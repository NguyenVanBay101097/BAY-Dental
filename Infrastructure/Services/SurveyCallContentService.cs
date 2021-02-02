using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SurveyCallContentService : BaseService<SurveyCallContent> , ISurveyCallContentService
    {
        readonly public IMapper _mapper;
        public SurveyCallContentService(IAsyncRepository<SurveyCallContent> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SurveyCallContentBasic>> GetPagedResultAsync(SurveyCallContentPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.AssignmentId.HasValue)
                query = query.Where(x => x.AssignmentId == val.AssignmentId.Value);

            var totalItems = await query.CountAsync();

            query = query.OrderBy(x => x.DateCreated);


            if(val.Limit != 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await query.ToListAsync();

            var paged = new PagedResult2<SurveyCallContentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SurveyCallContentBasic>>(items)
            };

            return paged;
        }


        public async Task<SurveyCallContentDisplay> GetDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<SurveyCallContentDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Nội dung cuộc gọi không tồn tại");

            return res;
        }

        public async Task<SurveyCallContent> CreateSurveyCallContent(SurveyCallContentSave val)
        {
            var callcontent = _mapper.Map<SurveyCallContent>(val);
          

            return await CreateAsync(callcontent);
        }

        public async Task UpdateSurveyCallContent(Guid id, SurveyCallContentSave val)
        {
            var callcontent = await SearchQuery(x => x.Id == id).Include(x => x.Assignment).FirstOrDefaultAsync();
            if (callcontent == null)
                throw new Exception("Nội dung cuộc gọi không tồn tại");

            callcontent = _mapper.Map(val, callcontent);

            await UpdateAsync(callcontent);
        }

    }
}
