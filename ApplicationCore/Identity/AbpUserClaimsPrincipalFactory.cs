using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Identity
{
    public class AbpUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }
        protected IAbpClaimsPrincipalFactory AbpClaimsPrincipalFactory { get; }

        public AbpUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options,
            ICurrentPrincipalAccessor currentPrincipalAccessor,
            IAbpClaimsPrincipalFactory abpClaimsPrincipalFactory) : base(userManager, roleManager, options)
        {
            CurrentPrincipalAccessor = currentPrincipalAccessor;
            AbpClaimsPrincipalFactory = abpClaimsPrincipalFactory;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = principal.Identities.First();

            identity.AddIfNotContains(new Claim(AbpClaimTypes.CompanyId, user.CompanyId.ToString()));

            if (!user.Name.IsNullOrWhiteSpace())
            {
                identity.AddIfNotContains(new Claim(AbpClaimTypes.Name, user.Name));
            }

            if (!user.PhoneNumber.IsNullOrWhiteSpace())
            {
                identity.AddIfNotContains(new Claim(AbpClaimTypes.PhoneNumber, user.PhoneNumber));
            }

            identity.AddIfNotContains(
                new Claim(AbpClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));

            if (!user.Email.IsNullOrWhiteSpace())
            {
                identity.AddIfNotContains(new Claim(AbpClaimTypes.Email, user.Email));
            }

            identity.AddIfNotContains(new Claim(AbpClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));

            using (CurrentPrincipalAccessor.Change(principal))
            {
                await AbpClaimsPrincipalFactory.CreateAsync(principal);
            }

            return principal;
        }
    }
}
