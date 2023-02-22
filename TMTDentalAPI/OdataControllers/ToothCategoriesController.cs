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
    public class ToothCategoriesController : BaseController
    {

        private readonly IToothCategoryService _toothCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ToothCategoriesController(IToothCategoryService toothCategoryService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _toothCategoryService = toothCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var result = _toothCategoryService.SearchQuery();
            return Ok(result);
        }

    }
}
