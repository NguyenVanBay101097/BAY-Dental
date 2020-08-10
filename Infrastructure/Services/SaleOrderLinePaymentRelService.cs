using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SaleOrderLinePaymentRelService : BaseService<SaleOrderLinePaymentRel>, ISaleOrderLinePaymentRelService
    {
        public SaleOrderLinePaymentRelService(IAsyncRepository<SaleOrderLinePaymentRel> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }
    }
}
