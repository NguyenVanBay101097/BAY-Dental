using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class MedicineOrderLineService : BaseService<MedicineOrderLine> , IMedicineOrderLineService
    {
        private readonly IMapper _mapper;
        public MedicineOrderLineService(IAsyncRepository<MedicineOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mappner)
            : base(repository, httpContextAccessor)
        {
            _mapper = mappner;
        }
    }
}
