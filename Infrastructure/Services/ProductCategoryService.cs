using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductCategoryService : BaseService<ProductCategory>, IProductCategoryService
    {
        private readonly IMapper _mapper;
        public ProductCategoryService(IAsyncRepository<ProductCategory> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            :base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ProductCategory> GetCategoryForDisplay(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Parent).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<ProductCategory>> GetPagedResultAsync(int pageIndex, int pageSize, string orderBy, string orderDirection, string filter = "")
        {
            Expression<Func<ProductCategory, object>> sort = null;
            switch(orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new ProductCategoryFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new ProductCategoryFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<ProductCategory>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<ProductCategoryBasic>> GetPagedResultAsync(ProductCategoryPaged val)
        {
            var query = GetQueryPaged(val);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (val.ServiceCateg.HasValue)
                query = query.Where(x => x.ServiceCateg == true);
            if (val.LaboCateg.HasValue)
                query = query.Where(x => x.LaboCateg == true);
            if (val.ProductCateg.HasValue)
                query = query.Where(x => x.ProductCateg == true);
            if (val.MedicineCateg.HasValue)
                query = query.Where(x => x.MedicineCateg == true);
            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);

            var items = await query.Skip(val.Offset).Take(val.Limit).Include(x => x.Parent)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ProductCategoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ProductCategoryBasic>>(items)
            };
        }


        public async Task<IEnumerable<ProductCategoryBasic>> GetAutocompleteAsync(ProductCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (val.ServiceCateg.HasValue)
                query = query.Where(x => x.ServiceCateg == true);
            if (val.LaboCateg.HasValue)
                query = query.Where(x => x.LaboCateg == true);
            if (val.ProductCateg.HasValue)
                query = query.Where(x => x.ProductCateg == true);
            if (val.MedicineCateg.HasValue)
                query = query.Where(x => x.MedicineCateg == true);
            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductCategoryBasic>>(items);
        }

        private IQueryable<ProductCategory> GetQueryPaged(ProductCategoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.CompleteName.Contains(val.Search));
            //if (val.ServiceCateg.HasValue)
            //    query = query.Where(x => x.ServiceCateg == val.ServiceCateg);
            //if (val.LaboCateg.HasValue)
            //    query = query.Where(x => x.LaboCateg == val.LaboCateg);
            //if (val.MedicineCateg.HasValue)
            //    query = query.Where(x => x.MedicineCateg == val.MedicineCateg);
            //if (val.ProductCateg.HasValue)
            //    query = query.Where(x => x.ProductCateg == val.ProductCateg);

            query = query.OrderBy(s => s.ParentLeft);
            return query;
        }

        public async Task<ProductCategory> DefaultCategory()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            return res;
        }

        public override async Task<IEnumerable<ProductCategory>> CreateAsync(IEnumerable<ProductCategory> self)
        {
            foreach (var categ in self)
            {
                var recursion = await _CheckRecursion(categ);
                if (!recursion)
                    throw new Exception("Không thể tạo nhóm sản phẩm đệ quy.");

                categ.CompleteName = await NameGet(categ);
            }

            await base.CreateAsync(self);

            await _ParentStoreCompute();
            return self;
        }

        public async Task Write(ProductCategory self)
        {
            if (self.Parent != null && self.Parent.Id != self.ParentId)
                self.Parent = await GetByIdAsync(self.ParentId);
            var recursion = await _CheckRecursion(self);
            if (!recursion)
                throw new Exception("Không thể tạo nhóm sản phẩm đệ quy.");
            self.CompleteName = await NameGet(self);

            await base.UpdateAsync(self);

            await _ParentStoreCompute();
        }

        public async Task<string> NameGet(ProductCategory categ)
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

        public async Task<ProductCategory> CreateCategByCompleteName(string completeName)
        {
            var categ = await SearchQuery(x => x.CompleteName == completeName).FirstOrDefaultAsync();
            if (categ != null)
                return categ;

            if (!completeName.Contains("/"))
            {
                return await CreateAsync(new ProductCategory()
                {
                    Name = completeName.Trim()
                });
            }

            var lastIndex = completeName.LastIndexOf(" / ");
            var categName = completeName.Substring(lastIndex + 3, completeName.Length - lastIndex - 3);
            var parentCompleteName = completeName.Substring(0, lastIndex);
            if (!string.IsNullOrEmpty(parentCompleteName))
                parentCompleteName = parentCompleteName.Trim();

            var parent = await CreateCategByCompleteName(parentCompleteName);

            return await CreateAsync(new ProductCategory()
            {
                Name = categName,
                ParentId = parent.Id,
                Parent = parent
            });
        }

        private async Task _ParentStoreCompute()
        {
            var query = await SearchQuery(x => !x.ParentId.HasValue, orderBy: x => x.OrderBy(s => s.Sequence).ThenBy(s => s.Name)).ToListAsync();
            var pos = 0;
            foreach (var root in query)
            {
                pos = await BrowseRec(root, pos);
            }
        }

        private async Task<int> BrowseRec(ProductCategory root, int pos)
        {
            Expression<Func<ProductCategory, bool>> where = x => x.ParentId == root.Id;
            var pos2 = pos + 1;
            var res = await SearchQuery(x => x.ParentId == root.Id, orderBy: x => x.OrderBy(s => s.Sequence).ThenBy(s => s.Name)).ToListAsync();
            foreach (var item in res)
                pos2 = await BrowseRec(item, pos2);

            root.ParentLeft = pos;
            root.ParentRight = pos2;
            await UpdateAsync(root);
            return pos2 + 1;
        }


        private async Task<bool> _CheckRecursion(ProductCategory entity)
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
    }
}
