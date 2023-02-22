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
    public class EmployeeCategoryService : BaseService<EmployeeCategory>, IEmployeeCategoryService
    {
        private readonly IMapper _mapper;
        public EmployeeCategoryService(IAsyncRepository<EmployeeCategory> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override async Task<EmployeeCategory> CreateAsync(EmployeeCategory entity)
        {
            var categ = await SearchQuery(x => x.Name == entity.Name).FirstOrDefaultAsync();
            if (categ != null)
                throw new Exception($"Đã tồn tại nhóm nhân viên với tên: {entity.Name}");
            return await base.CreateAsync(entity);
        }

        public override async Task UpdateAsync(EmployeeCategory entity)
        {
            var categ = await SearchQuery(x => x.Name == entity.Name).FirstOrDefaultAsync();
            //if (categ != null)
            //    throw new Exception($"Đã tồn tại nhóm nhân viên với tên: {entity.Name}");
            await base.UpdateAsync(entity);
        }

        private IQueryable<EmployeeCategory> GetQueryPaged(EmployeeCategoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public async Task<PagedResult2<EmployeeCategoryBasic>> GetPagedResultAsync(EmployeeCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<EmployeeCategoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<EmployeeCategoryBasic>>(items)
            };
        }

        public async Task<IEnumerable<EmployeeCategoryBasic>> GetAutocompleteAsync(EmployeeCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeCategoryBasic>>(items);
        }

        public async Task<IEnumerable<EmployeeCategoryDisplay>> GetAutocompleteAsync2(EmployeeCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeCategoryDisplay>>(items);
        }
    }
}
