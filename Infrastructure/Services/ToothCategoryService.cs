using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ToothCategoryService : BaseService<ToothCategory>, IToothCategoryService
    {
        private readonly IMapper _mapper;
        public ToothCategoryService(IAsyncRepository<ToothCategory> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<ToothCategoryBasic>> GetAllBasic()
        {
            var categories = await SearchQuery(orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            var res = _mapper.Map<IEnumerable<ToothCategoryBasic>>(categories);
            return res;
        }

        public async Task<ToothCategoryBasic> GetDefaultCategory()
        {
            var category = await SearchQuery(orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
            var res = _mapper.Map<ToothCategoryBasic>(category);
            return res;
        }

        public async Task<PagedResult2<ToothCategory>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "")
        {
            Expression<Func<ToothCategory, bool>> domain = x => string.IsNullOrEmpty(search) || x.Name.Contains(search);

            var query = SearchQuery(domain: domain, orderBy: x => x.OrderBy(s => s.Name), offSet: offset, limit: limit);
            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ToothCategory>(totalItems, offset, limit)
            {
                Items = items
            };
        }
    }
}
