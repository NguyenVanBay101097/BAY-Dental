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
    public class FacebookTagService : BaseService<FacebookTag>, IFacebookTagService
    {
        private readonly IMapper _mapper;
        public FacebookTagService(IAsyncRepository<FacebookTag> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<PagedResult2<FacebookTagBasic>> GetPagedResultAsync(SimplePaged val)
        {
            var query = SearchQuery(x => string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search));

            var items = await _mapper.ProjectTo<FacebookTagBasic>(query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookTagBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task CheckUniqueName(FacebookTag self)
        {
            var exist = await SearchQuery(x => x.Name == self.Name).FirstOrDefaultAsync();
            if (exist != null)
                throw new Exception($"Đã tồn tại nhãn '{self.Name}'");
        }

        public override async Task<IEnumerable<FacebookTag>> CreateAsync(IEnumerable<FacebookTag> entities)
        {
            foreach (var entity in entities)
                await CheckUniqueName(entity);

            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<FacebookTag> entities)
        {
            foreach (var entity in entities)
                await CheckUniqueName(entity);

            await base.UpdateAsync(entities);
        }
    }
}
