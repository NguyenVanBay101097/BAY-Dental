using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PaymentQuotationService : BaseService<PaymentQuotation>, IPaymentQuotationService
    {
        public PaymentQuotationService(IAsyncRepository<PaymentQuotation> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }
    }
}
