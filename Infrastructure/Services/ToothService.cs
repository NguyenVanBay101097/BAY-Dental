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
    public class ToothService : BaseService<Tooth>, IToothService
    {
        private readonly IMapper _mapper;

        public ToothService(IAsyncRepository<Tooth> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<ToothDisplay>> GetAllDisplay(ToothFilter val)
        {
            var teeth = await _mapper.ProjectTo<ToothDisplay>(SearchQuery(domain: x => !val.CategoryId.HasValue || x.CategoryId == val.CategoryId,
                orderBy: x => x.OrderBy(s => s.Name)).Include(x => x.Category)).ToListAsync();
            return teeth;
        }

        public async Task<PagedResult2<Tooth>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "")
        {
            Expression<Func<Tooth, bool>> domain = x => string.IsNullOrEmpty(search) || x.Name.Contains(search);

            var query = SearchQuery(domain: domain, orderBy: x => x.OrderBy(s => s.Name), offSet: offset, limit: limit);
            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<Tooth>(totalItems, offset, limit)
            {
                Items = items
            };
        }
    }
}
