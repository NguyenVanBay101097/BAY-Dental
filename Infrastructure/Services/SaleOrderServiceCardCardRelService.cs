using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SaleOrderServiceCardCardRelService : BaseService<SaleOrderServiceCardCardRel>, ISaleOrderServiceCardCardRelService
    {
        public SaleOrderServiceCardCardRelService(IAsyncRepository<SaleOrderServiceCardCardRel> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
