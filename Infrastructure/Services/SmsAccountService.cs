using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class SmsAccountService : BaseService<SmsAccount>, ISmsAccountService
    {
        private readonly IMapper _mapper;
        public SmsAccountService(IMapper mapper, IAsyncRepository<SmsAccount> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SmsAccountBasic> GetDefault()
        {
            var smsAccount = await SearchQuery().OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            return _mapper.Map<SmsAccountBasic>(smsAccount);
        }
    }
}
