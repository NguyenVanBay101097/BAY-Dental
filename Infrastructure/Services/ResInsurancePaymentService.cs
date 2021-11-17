using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
   public class ResInsurancePaymentService : BaseService<ResInsurancePayment> , IResInsurancePaymentService
    {
        private readonly IMapper _mapper;

        public ResInsurancePaymentService(IAsyncRepository<ResInsurancePayment> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override ISpecification<ResInsurancePayment> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.res_insurance_payment_comp_rule":
                    return new InitialSpecification<ResInsurancePayment>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
