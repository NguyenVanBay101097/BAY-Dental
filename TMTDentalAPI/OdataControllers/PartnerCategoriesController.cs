using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class PartnerCategoriesController : BaseController
    {
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IMapper _mapper;

        public PartnerCategoriesController(IPartnerCategoryService partnerCategoryService,
            IMapper mapper)
        {
            _partnerCategoryService = partnerCategoryService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _partnerCategoryService.GetViewModelsAsync();
            return Ok(results);
        }
    }
}
