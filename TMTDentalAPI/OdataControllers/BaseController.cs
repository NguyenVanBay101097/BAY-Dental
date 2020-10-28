using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.OdataControllers
{
    //[Route("odata/[controller]")]
    //[Authorize]
    [ApiController]
    public class BaseController : ODataController
    {
        protected string UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return null;

                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        protected Guid CompanyId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }
    }
}
