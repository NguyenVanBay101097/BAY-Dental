using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ApplicationUsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUsersController(IUserService userService,
            UserManager<ApplicationUser> userManager,
           IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _userManager = userManager;
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
        [CheckAccess(Actions = "System.ApplicationUser.Update")]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<ApplicationUserViewModel>(_userManager.Users);
            return Ok(results);
        }
    }
}
