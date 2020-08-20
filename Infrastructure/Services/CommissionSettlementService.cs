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
    public class CommissionSettlementService : BaseService<CommissionSettlement>, ICommissionSettlementService
    {
        private readonly IMapper _mapper;
        public CommissionSettlementService(IAsyncRepository<CommissionSettlement> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper ) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

      

    }
}
