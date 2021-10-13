using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PublicApisController : BaseApiController
    {
        [HttpGet]
        public IActionResult FakeData()
        {
            var categ = new List<ProductCategory>
        {
            new ProductCategory
            {
                Id = Guid.NewGuid(), Name = "John Doe",
              
            },
            new ProductCategory
            {
                Id =  Guid.NewGuid(),
                Name = "Elizabeth Johnson",
            },
            new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = "Bob Rogers",
            }
        };

            return Ok(categ);
        }
    }
}
