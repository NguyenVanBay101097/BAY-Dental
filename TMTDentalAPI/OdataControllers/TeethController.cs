using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class TeethController : BaseController
    {
        private readonly IToothService _toothService;
        private readonly IMapper _mapper;
        public TeethController(IToothService toothService,
                 IMapper mapper)
        {
            _toothService = toothService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var result = _toothService.SearchQuery();
            return Ok(result);
        }
    }
}
