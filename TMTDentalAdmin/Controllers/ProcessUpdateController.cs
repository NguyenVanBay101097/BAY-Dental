using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessUpdateController : BaseApiController
    {
        private readonly ITenantService _tenantService;
        private readonly AdminAppSettings _appSettings;

        public ProcessUpdateController(ITenantService tenantService,
            IOptions<AdminAppSettings> appSettings)
        {
            _tenantService = tenantService;
            _appSettings = appSettings?.Value;
        }
    }
}
