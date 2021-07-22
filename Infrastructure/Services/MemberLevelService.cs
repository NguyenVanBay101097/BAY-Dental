using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class MemberLevelService : BaseService<MemberLevel>, IMemberLevelService
    {
        private IMapper _mapper;
        public MemberLevelService(IAsyncRepository<MemberLevel> repository, IHttpContextAccessor httpContextAccessor,
           IMapper mapper
           )
       : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<MemberLevelSimple>> AutoComplete(MemberLevelAutoCompleteReq val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var count = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);
            var res = await query.ToListAsync();
            return new PagedResult2<MemberLevelSimple>(count, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<MemberLevelSimple>>(res)
            };
        }

        public async Task<IEnumerable<MemberLevel>> Get()
        {
            var result = await SearchQuery(x => x.CompanyId == CompanyId).ToListAsync();
            return result;
        }

        public async Task UpdateMember(IEnumerable<MemberLevelSave> vals)
        {
            var listDb = await SearchQuery(x => x.CompanyId == CompanyId).ToListAsync();
            var memberAdd = new List<MemberLevel>();
            var memberUpdate = new List<MemberLevel>();
            var memberDelete = new List<MemberLevel>();

            foreach (var item in listDb)
            {
                if (!vals.Any(x => x.Id == item.Id))
                {
                    memberDelete.Add(item);
                }
            }

            foreach (var item in vals)
            {
                if (!listDb.Any(x => x.Id == item.Id))
                {
                    var entity = _mapper.Map<MemberLevel>(item);
                    entity.CompanyId = CompanyId;
                    memberAdd.Add(entity);
                }
                else
                {
                    var entity = await SearchQuery(x => x.Id == item.Id).FirstOrDefaultAsync();
                    entity = _mapper.Map(item, entity);
                    entity.CompanyId = CompanyId;
                    memberUpdate.Add(entity);
                }
            }

            await DeleteAsync(memberDelete);
            await UpdateAsync(memberUpdate);
            await CreateAsync(memberAdd);
            
        }

      
    }
}
