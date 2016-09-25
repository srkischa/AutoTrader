using AutoMapper;
using AutoTrader.Service;
using AutoTrader.Service.Identity;
using AutoTrader.WebApi.Request;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AutoTrader.WebApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IMapper _mapper;
        private readonly IUserIdentityManagerService _userManagementService;
        private readonly IUserIdentityManagerService _userManager;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IConfigurationSettings _configurationSettings;

        public AccountController(
            IUserIdentityManagerService userManager,
            IAuthenticationManager authenticationManager,
            IUserIdentityManagerService userManagementService,
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IConfigurationSettings configurationSettings)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
            _userManagementService = userManagementService;
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _configurationSettings = configurationSettings;
        }

        [HttpPost]
        [Route("register-user")]
        public async Task<IHttpActionResult> Register(UserRegistrationRequest model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Email);
            if (existingUser != null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("User with that email alerady exist")
                };
                throw new HttpResponseException(errorResponse);
            }

            var user = _mapper.Map<ApplicationUser>(model);

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            var errorResult = GetErrorResult(identityResult);
            if (errorResult != null) return errorResult;

            identityResult = await _userManager.AddToRolesAsync(user.Id, "BasicUser");

            errorResult = GetErrorResult(identityResult);
            if (errorResult != null) return errorResult;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = new Uri($"{_configurationSettings.FrontEndUrl}confirm?userId={user.Id}&code={code}");

            string emailTitle = "Please confirm your account";
            string emailBody = callbackUrl.AbsoluteUri;

            await _userManager.SendEmailAsync(user.Id, emailTitle, emailBody);

            return Ok();
        }

        [HttpGet]
        [Route("register-email")]
        public async Task<IHttpActionResult> ConfirmEmail(int userId, string code)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(userId, code);
            var errorResult = GetErrorResult(identityResult);
            return errorResult ?? Ok();
        }

        [Authorize]
        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest model)
        {
            var result = await _userManager.ChangePasswordAsync(_currentUserProvider.User.Id, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forget-password")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Not allowed!")
                };
                throw new HttpResponseException(errorResponse);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            var callbackUrl = new Uri($"{_configurationSettings.FrontEndUrl}reset-password?email={model.Email}&code={code}");

            string emailTitle = "Please reset your password";
            string emailBody = callbackUrl.AbsoluteUri;

            await _userManager.SendEmailAsync(user.Id, emailTitle, emailBody);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IHttpActionResult> ResetPassword(RequestPasswordRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Not allowed!")
                };
                throw new HttpResponseException(errorResponse);
            }

            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
