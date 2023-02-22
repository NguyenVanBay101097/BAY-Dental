using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ResInsurancePaymentLineService : BaseService<ResInsurancePaymentLine>, IResInsurancePaymentLineService
    {
        public ResInsurancePaymentLineService(IAsyncRepository<ResInsurancePaymentLine> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }
    }
}
