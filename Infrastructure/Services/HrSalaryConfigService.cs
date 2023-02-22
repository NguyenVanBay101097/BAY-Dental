using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Hangfire.States;
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
    public class HrSalaryConfigService : BaseService<HrSalaryConfig>, IHrSalaryConfigService
    {
        private readonly IMapper _mapper;
        public HrSalaryConfigService(IAsyncRepository<HrSalaryConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrSalaryConfigDisplay> GetByCompanyId(Guid companyId)
        {
            var res = await _mapper.ProjectTo<HrSalaryConfigDisplay>(SearchQuery(x => x.CompanyId == companyId)).FirstOrDefaultAsync();
            if (res == null)
                res = new HrSalaryConfigDisplay() { CompanyId = companyId };
            return res;
        }

    }
}
