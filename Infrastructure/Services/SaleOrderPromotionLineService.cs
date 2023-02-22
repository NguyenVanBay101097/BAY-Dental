using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SaleOrderPromotionLineService : BaseService<SaleOrderPromotionLine>, ISaleOrderPromotionLineService
    {
        private readonly IMapper _mapper;

        public SaleOrderPromotionLineService(IAsyncRepository<SaleOrderPromotionLine> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
