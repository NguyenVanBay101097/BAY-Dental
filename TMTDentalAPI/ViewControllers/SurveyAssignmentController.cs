using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.ViewControllers
{
    public class SurveyAssignmentController : Controller
    {
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SurveyAssignmentController(IViewToStringRenderService viewToStringRenderService, IMapper mapper)
        {
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
