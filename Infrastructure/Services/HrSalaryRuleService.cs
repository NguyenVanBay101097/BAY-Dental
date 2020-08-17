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
   public class HrSalaryRuleService : BaseService<HrSalaryRule>, IHrSalaryRuleService
    {

        private readonly IMapper _mapper;
        public HrSalaryRuleService(IAsyncRepository<HrSalaryRule> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task Remove(IEnumerable<Guid> Ids)
        {
            var list = await SearchQuery(x=>Ids.Contains(x.Id)).ToListAsync();
            await DeleteAsync(list);
        }
    }
}
