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
    public class SurveyTagService : BaseService<SurveyTag> , ISurveyTagService
    {
        private readonly IMapper _mapper;
        public SurveyTagService(IAsyncRepository<SurveyTag> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SurveyTagBasic>> GetPagedResultAsync(SurveyTagPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
        
            var totalItems = await query.CountAsync();

            query = query.OrderBy(x => x.DateCreated);


            if (val.Limit != 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await query.ToListAsync();

            var paged = new PagedResult2<SurveyTagBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SurveyTagBasic>>(items)
            };

            return paged;
        }


        //public async Task<SurveyTag> CreateSurveyTag(SurveyTagSave val)
        //{

        //}

        //public async Task UpdateSurveyTag(Guid id, SurveyTagSave val)
        //{

        //}

    }
}
