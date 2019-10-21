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
    public class PartnerCategoryService : BaseService<PartnerCategory>, IPartnerCategoryService
    {
        private readonly IMapper _mapper;

        public PartnerCategoryService(IAsyncRepository<PartnerCategory> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PartnerCategory> CreatePartnerCategoryAsync(PartnerCategory categ)
        {
            var recursion = await _CheckRecursion(categ);
            if (!recursion)
                throw new Exception("Không thể tạo nhóm sản phẩm đệ quy.");
            categ.CompleteName = await NameGet(categ);
            await base.CreateAsync(categ);

            await _ParentStoreCompute();
            return categ;
        }

        public async Task UpdatePartnerCategoryAsync(PartnerCategory categ)
        {
            var recursion = await _CheckRecursion(categ);
            if (!recursion)
                throw new Exception("Không thể tạo nhóm sản phẩm đệ quy.");
            categ.CompleteName = await NameGet(categ);
            await base.UpdateAsync(categ);

            await _ParentStoreCompute();
        }

        public async Task<IEnumerable<PartnerCategoryBasic>> GetAutocompleteAsync(PartnerCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Include(x => x.Parent).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PartnerCategoryBasic>>(items);
        }

        private async Task _ParentStoreCompute()
        {
            var query = await SearchQuery(x => !x.ParentId.HasValue, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            var pos = 0;
            foreach (var root in query)
            {
                pos = await BrowseRec(root, pos);
            }
        }

        private async Task<int> BrowseRec(PartnerCategory root, int pos)
        {
            var pos2 = pos + 1;
            var res = await SearchQuery(x => x.ParentId == root.Id, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            foreach (var item in res)
                pos2 = await BrowseRec(item, pos2);

            root.ParentLeft = pos;
            root.ParentRight = pos2;
            await UpdateAsync(root);
            return pos2 + 1;
        }

        public async Task<string> NameGet(PartnerCategory categ)
        {
            var res = new List<string>();
            while (categ != null)
            {
                res.Add(categ.Name);
                if (categ.ParentId.HasValue && categ.Parent == null)
                    categ.Parent = await GetByIdAsync(categ.ParentId);
                categ = categ.Parent;
            }

            res.Reverse();

            return string.Join(" / ", res);
        }

        private async Task<bool> _CheckRecursion(PartnerCategory entity)
        {
            var currentId = entity.ParentId;
            while (currentId.HasValue)
            {
                currentId = (await GetByIdAsync(currentId.Value)).ParentId;
                if (currentId == entity.Id)
                    return false;
            }

            return true;
        }

        private IQueryable<PartnerCategory> GetQueryPaged(PartnerCategoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.CompleteName.Contains(val.Search));

            query = query.OrderBy(s => s.ParentLeft);
            return query;
        }

        public async Task<PagedResult2<PartnerCategoryBasic>> GetPagedResultAsync(PartnerCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<PartnerCategoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<PartnerCategoryBasic>>(items)
            };
        }
    }
}
