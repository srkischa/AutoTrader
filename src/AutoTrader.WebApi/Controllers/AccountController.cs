using AutoMapper;
using AutoTrader.Service.Identity;
using AutoTrader.WebApi.Request;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace AutoTrader.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IUserIdentityManagerService _userManager;

        public AccountController(
            IUserIdentityManagerService userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Register(UserRegistrationRequest model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Email);

            if (existingUser != null) throw new Exception("User with that email alerady exist");

            var user = _mapper.Map<ApplicationUser>(model);

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            //SendNotificationEmail(model.Email, user);

            var errorResult = GetErrorResult(identityResult);

            if (errorResult != null)
            {
                return errorResult;
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
