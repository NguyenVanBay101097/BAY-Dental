using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SaleProductionLineService : BaseService<SaleProductionLine> , ISaleProductionLineService
    {
        private readonly IMapper _mapper;

        public SaleProductionLineService(IAsyncRepository<SaleProductionLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {

        }
    }
}
