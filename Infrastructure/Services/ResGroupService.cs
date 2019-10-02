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
    public class ResGroupService : BaseService<ResGroup>, IResGroupService
    {
        private readonly IMapper _mapper;

        public ResGroupService(IAsyncRepository<ResGroup> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        private IQueryable<ResGroup> GetQueryPaged(ResGroupPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public async Task<PagedResult2<ResGroupBasic>> GetPagedResultAsync(ResGroupPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ResGroupBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResGroupBasic>>(items)
            };
        }

        public async Task<ResGroupDisplay> DefaultGet()
        {
            //lấy hết những ir model access ra add vào cho group, để người dùng chỉ cần check 
            //sẽ dùng những ir model access nào active = false làm dữ liệu mẫu add cho group
            var res = new ResGroupDisplay();
            var accessObj = GetService<IIRModelAccessService>();
            var accesses = await accessObj.SearchQuery(x => !x.GroupId.HasValue && !x.Active, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            var list = new List<IRModelAccessDisplay>();
            foreach(var access in accesses)
            {
                list.Add(new IRModelAccessDisplay
                {
                    Name = access.Name,
                    PermRead = access.PermRead,
                    PermCreate = access.PermCreate,
                    PermWrite = access.PermWrite,
                    PermUnlink = access.PermUnlink,
                    ModelId = access.ModelId
                });
            }
            res.ModelAccesses = list;
            return res;
        }

        public override Task<ResGroup> CreateAsync(ResGroup entity)
        {

            return base.CreateAsync(entity);
        }
    }
}
