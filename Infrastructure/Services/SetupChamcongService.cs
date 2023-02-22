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

namespace Infrastructure.Services
{
    public class SetupChamcongService : BaseService<SetupChamcong>, ISetupChamcongService
    {
        private readonly IMapper _mapper;
        public SetupChamcongService(IAsyncRepository<SetupChamcong> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SetupChamcong> GetByCompanyId(Guid? companyId=null)
        {
            if (companyId == null)
            {
                companyId = this.CompanyId;
            }
            return await SearchQuery(x => x.CompanyId == companyId).FirstOrDefaultAsync();
        }

    }
}
