using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResInsuranceReportsController : BaseApiController
    {
        private readonly IResInsuranceReportService _resInsuranceReportService;
        private readonly IMapper _mapper;

        public ResInsuranceReportsController(IResInsuranceReportService resInsuranceReportService, IMapper mapper)
        {
            _resInsuranceReportService = resInsuranceReportService;
            _mapper = mapper;
        }


    }
}
