using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ProductRequestLineService : BaseService<ProductRequestLine> , IProductRequestLineService
    {
        private readonly IMapper _mapper;
        public ProductRequestLineService(IAsyncRepository<ProductRequestLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
