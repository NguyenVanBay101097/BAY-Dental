using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class QuotationPromotionLineService : BaseService<QuotationPromotionLine>, IQuotationPromotionLineService
    {
        private readonly IMapper _mapper;
        public QuotationPromotionLineService(IAsyncRepository<QuotationPromotionLine> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
