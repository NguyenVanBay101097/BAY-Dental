using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SamplePrescriptionLineService : BaseService<SamplePrescriptionLine>, ISamplePrescriptionLineService
    {
        public SamplePrescriptionLineService(IAsyncRepository<SamplePrescriptionLine> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }
    }
}
