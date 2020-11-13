using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController
    {

        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public EmployeesController(IEmployeeService employeeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get(ODataQueryOptions<Employee> options)
        {
            var result = _employeeService.SearchQuery();
            return Ok(result);
        }

    }
}
