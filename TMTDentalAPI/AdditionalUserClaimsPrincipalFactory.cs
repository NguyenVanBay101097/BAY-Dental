using ApplicationCore.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TMTDentalAPI
{
    public class AdditionalUserClaimsPrincipalFactory
          : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected async override Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var principal = await base.GenerateClaimsAsync(user);
            var claims = new List<Claim>
            {
                new Claim("company", user.CompanyId.ToString()),
            };

            principal.AddClaims(claims);
            return principal;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>
            {
                new Claim("company", user.CompanyId.ToString()),
            };

            identity.AddClaims(claims);
            return principal;
        }
    }
}