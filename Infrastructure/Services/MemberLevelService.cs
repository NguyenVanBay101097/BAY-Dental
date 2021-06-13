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

        public async Task<IEnumerable<MemberLevel>> Get()
        {
            var result = await SearchQuery(x => x.CompanyId == CompanyId).ToListAsync();
            return result;
        }

        public async Task UpdateMember(IEnumerable<MemberLevelSave> vals)
        {
            var listDb = await SearchQuery().ToListAsync();
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
