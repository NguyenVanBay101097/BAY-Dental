using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SmsSmsService : BaseService<SmsSms>, ISmsSmsService
    {
        private readonly IMapper _mapper;

        public SmsSmsService(IMapper mapper, IAsyncRepository<SmsSms> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
