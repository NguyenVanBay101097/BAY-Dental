using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UoMService : BaseService<UoM>, IUoMService
    {

        public UoMService(IAsyncRepository<UoM> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<UoM> DefaultUOM()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            return res;
        }
    }
}
