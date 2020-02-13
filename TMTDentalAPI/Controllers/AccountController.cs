using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _appSettings;
        private readonly AppTenant _tenant;
        private readonly IMailSender _mailSender;
        private readonly IAsyncRepository<UserRefreshToken> _userRefreshTokenRepository;
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;
        private readonly BirthdayMessageJobService _birthdayMessageJobService;

        private readonly CatalogDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<AppSettings> appSettings, ITenant<AppTenant> tenant,
            IMailSender mailSender, IAsyncRepository<UserRefreshToken> userRefreshTokenRepository,
            IUserService userService, IPartnerService partnerService, BirthdayMessageJobService birthdayMessageJobService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _tenant = tenant?.Value;
            _mailSender = mailSender;
            _userRefreshTokenRepository = userRefreshTokenRepository;
            _userService = userService;
            _partnerService = partnerService;
            _birthdayMessageJobService = birthdayMessageJobService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var _authenticationResult = new LoggedInViewModel
            {
                Succeeded = false,
            };

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.Users.Where(x => x.UserName == model.UserName).Include(x => x.Partner).FirstOrDefaultAsync();
                    var refreshToken = await GenerateRefreshToken(user);
                    var roles = new List<string>();

                    _authenticationResult = new LoggedInViewModel
                    {
                        Succeeded = true,
                        Message = "Authentication succeeded",
                        Token = GenerateToken(user, DateTime.Now.AddDays(7), roles),
                        RefreshToken = refreshToken.Token,
                        User = new UserViewModel
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Phone = user.PhoneNumber,
                            Email = user.Email,
                            Name = user.Name,
                            Avatar = user.Partner.Avatar
                        }
                    };
                }
                else
                {
                    _authenticationResult = new LoggedInViewModel
                    {
                        Succeeded = false,
                        Message = "Authentication failed",
                    };
                }
            }
            catch (Exception ex)
            {
                _authenticationResult = new LoggedInViewModel
                {
                    Succeeded = false,
                    Message = ex.Message
                };
            }

            return Ok(_authenticationResult);
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<ActionResult> Refresh([FromBody]RefreshViewModel model)
        {
            var refreshToken = await _userRefreshTokenRepository.SearchQuery(x => x.Token == model.RefreshToken)
                .Include(x => x.User).FirstOrDefaultAsync();
            if (refreshToken == null)
                return BadRequest();
            if (refreshToken.Expiration.HasValue && refreshToken.Expiration < DateTime.Now)
                return Unauthorized();
            if (!await _signInManager.CanSignInAsync(refreshToken.User))
                return Unauthorized();
            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(refreshToken.User))
                return Unauthorized();

            var response = new RefreshResponseViewModel()
            {
                AccessToken = GenerateToken(refreshToken.User, DateTime.Now.AddDays(7)),
                RefreshToken = refreshToken.Token
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult AddJob()
        {
            var host = _tenant.Hostname;
            RecurringJob.AddOrUpdate($"{host}-birthday2", () => _birthdayMessageJobService.SendMessage(host), "39 9 * * *");
            RecurringJob.AddOrUpdate($"{host}-birthday2", () => _birthdayMessageJobService.SendMessage(host), "39 2 * * *");
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.Email);
            }

            if (user == null)
            {
                return Ok(new { success = false, message = "Email không tồn tại!" });
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = System.Web.HttpUtility.UrlEncode(code);
            //string callbackUrl = Url.Action("resetpassword", "account", new { token = code }, Request.Scheme);
            string callbackUrl = Request.Scheme + ":" + Request.Host.Value + "/reset-password/" + code;
            await _mailSender.SendEmailAsync(user.Email, "Yêu cầu reset mật khẩu!", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return Ok(new { success = true, message = "Hệ thống đã gửi đường link để reset mật khẩu vào hộp thư của bạn, bạn hãy kiểm tra thư để reset mật khẩu!" });
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    return Ok(new
                    {
                        success = result.Succeeded,
                        message = string.Join("; ", result.Errors.Select(x => x.Description))
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        success = false,
                        message = ex.Message,
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))),
                });
            }
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new Exception("Không tìm thấy tài khoản với email " + model.Email);
            }
            //model.Code = System.Web.HttpUtility.UrlDecode(model.Code);
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Reset mật khẩu không thành công.");
            }

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetGroups()
        {
            var res = await _userService.GetGroups();
            return Ok(res);
        }

        private string GenerateToken(ApplicationUser user, DateTime expires, IList<string> roles = null)
        {
            if (roles == null)
                roles = new List<string>();
            var handler = new JwtSecurityTokenHandler();

            //ClaimsIdentity identity = new ClaimsIdentity(
            //    new Claim[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }
            //);

            ClaimsIdentity identity = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("company_id", user.CompanyId.ToString()),
                    new Claim("user_root", user.IsUserRoot.ToString())
               }
           );

            //identity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            //

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret + (_tenant != null ? _tenant.Hostname : ""));
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = identity,
                Expires = expires,
            });
            return handler.WriteToken(securityToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<UserRefreshToken> GenerateRefreshToken(ApplicationUser user)
        {
            // Create the refresh token
            var refreshToken = new UserRefreshToken()
            {
                Token = GenerateRefreshToken(),
                Expiration = DateTime.Now.AddMonths(3) // Make this configurable
            };

            // Add it to the list of of refresh tokens for the user
            user.RefreshTokens.Add(refreshToken);

            // Update the user along with the new refresh token
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetUserInfo(string id)
        {
            var entity = await _userManager.Users.Where(x => x.Id == id).Include(x=>x.Partner).FirstOrDefaultAsync();
            var user = new UserInfo
            {
                Name = entity.Name,
                Avatar = entity.Partner.Avatar
            };

            return Ok(user);
        }
    }

    class UserInfo
    {
        public string Avatar { get; set; }
        public string Name { get; set; }
    }
}