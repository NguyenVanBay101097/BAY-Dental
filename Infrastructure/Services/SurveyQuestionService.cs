﻿using ApplicationCore.Entities;
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
    public class SurveyQuestionService: BaseService<SurveyQuestion>, ISurveyQuestionService
    {
        readonly public IMapper _mapper;
        public SurveyQuestionService(IAsyncRepository<SurveyQuestion> repository, IHttpContextAccessor httpContextAccessor,
             IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SurveyQuestionBasic>> GetPagedResultAsync(SurveyQuestionPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            if (!string.IsNullOrEmpty(val.Type))
            {
                query = query.Where(x=> x.Type == val.Type);
            }

            var count = await query.CountAsync();
            query = query.Skip(val.Offset);
            if (val.Limit > 0)
                query = query.Take(val.Limit);
            var items = await query.OrderBy(x=> x.Sequence).ToListAsync();

            return new PagedResult2<SurveyQuestionBasic>(count, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SurveyQuestionBasic>>(items)
            };
        }
    }
}
