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
    public class LaboBiteJointService : BaseService<LaboBiteJoint>, ILaboBiteJointService
    {
        private IMapper _mapper;
        public LaboBiteJointService(IAsyncRepository<LaboBiteJoint> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboBiteJointBasic>> GetPagedResultAsync(LaboBiteJointsPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.ToLower().Contains(val.Search.Trim().ToLower()));

            var items = await _mapper.ProjectTo<LaboBiteJointBasic>(query.OrderBy(x => x.Name).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<LaboBiteJointBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<LaboBiteJointDisplay> GetDisplay(Guid id)
        {
            var query = SearchQuery(x => x.Id == id);
            var res = await _mapper.ProjectTo<LaboBiteJointDisplay>(query).FirstOrDefaultAsync();
            return res;
        }

        public async Task<LaboBiteJointDisplay> CreateItem(LaboBiteJointSave val)
        {
            var item = _mapper.Map<LaboBiteJoint>(val);
            await CreateAsync(item);
            return _mapper.Map<LaboBiteJointDisplay>(item);
        }

        public async Task UpdateItem(Guid id, LaboBiteJointSave val)
        {
            var item = SearchQuery().Where(x => x.Id == id).FirstOrDefault();
            if (item == null) throw new Exception("Không tồn tại đường hoàn tất");
            item = _mapper.Map(val,item);
            await UpdateAsync(item);
        }

        public async Task<IEnumerable<LaboBiteJointSimple>> Autocomplete(LaboBiteJointPageSimple val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(val.Search.Trim().ToLower()));
            }

            if (val.Offset.HasValue)
            {
                query = query.Skip(val.Limit.Value);
            }

            if (val.Limit.HasValue && val.Limit.Value > 0)
            {
                query = query.Take(val.Limit.Value);
            }

            return await query.OrderBy(x => x.Name).Select(x => new LaboBiteJointSimple
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
