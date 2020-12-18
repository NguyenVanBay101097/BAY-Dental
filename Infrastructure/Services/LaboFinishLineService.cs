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
    public class LaboFinishLineService : BaseService<LaboFinishLine>, ILaboFinishLineService
    {
        private IMapper _mapper;
        public LaboFinishLineService(IAsyncRepository<LaboFinishLine> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboFinishLineBasic>> GetPagedResultAsync(LaboFinishLinesPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.ToLower().Contains(val.Search.Trim().ToLower()));

            var items = await _mapper.ProjectTo<LaboFinishLineBasic>(query.OrderBy(x => x.Name).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<LaboFinishLineBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<LaboFinishLineDisplay> GetDisplay(Guid id)
        {
            var query = SearchQuery(x => x.Id == id);
            var res = await _mapper.ProjectTo<LaboFinishLineDisplay>(query).FirstOrDefaultAsync();
            return res;
        }

        public async Task<LaboFinishLineDisplay> CreateItem(LaboFinishLineSave val)
        {
            var item = _mapper.Map<LaboFinishLine>(val);
            await CreateAsync(item);
            return _mapper.Map<LaboFinishLineDisplay>(item);
        }

        public async Task UpdateItem(Guid id, LaboFinishLineSave val)
        {
            var item = SearchQuery().Where(x => x.Id == id).FirstOrDefault();
            if (item == null) throw new Exception("Không tồn tại đường hoàn tất");
            item = _mapper.Map(val,item);
            await UpdateAsync(item);
        }
    }
}
