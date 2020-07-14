using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PhieuThuChiService : BaseService<PhieuThuChi>, IPhieuThuChiService
    {
        private readonly IMapper _mapper;

        public PhieuThuChiService(IAsyncRepository<PhieuThuChi> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
