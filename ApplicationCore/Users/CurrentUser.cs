using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Volo.Abp.Security.Claims;
using System.Linq;
using System.Security.Principal;

namespace ApplicationCore.Users
{
    public class CurrentUser : ICurrentUser
    {
        private static readonly Claim[] EmptyClaimsArray = new Claim[0];

        public virtual bool IsAuthenticated => !Id.IsNullOrWhiteSpace();

        public virtual string Id => _principalAccessor.Principal?.FindUserId();

        public virtual string UserName => this.FindClaimValue(AbpClaimTypes.UserName);

        public virtual string Name => this.FindClaimValue(AbpClaimTypes.Name);

        public virtual string SurName => this.FindClaimValue(AbpClaimTypes.SurName);

        public virtual string PhoneNumber => this.FindClaimValue(AbpClaimTypes.PhoneNumber);

        public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

        public virtual string Email => this.FindClaimValue(AbpClaimTypes.Email);

        public virtual bool EmailVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);

        public virtual Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

        public virtual string[] Roles => FindClaims(AbpClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

        public Guid? CompanyId => this.FindClaimValue<Guid>(AbpClaimTypes.CompanyId);

        private readonly ICurrentPrincipalAccessor _principalAccessor;

        public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public virtual Claim FindClaim(string claimType)
        {
            return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
        }

        public virtual Claim[] GetAllClaims()
        {
            return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
        }

        public virtual bool IsInRole(string roleName)
        {
            return FindClaims(AbpClaimTypes.Role).Any(c => c.Value == roleName);
        }
    }
}
