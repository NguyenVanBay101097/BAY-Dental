using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SaleProductionService : BaseService<SaleProduction> , ISaleProductionService
    {
        private readonly IMapper _mapper;

        public SaleProductionService(IAsyncRepository<SaleProduction> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
