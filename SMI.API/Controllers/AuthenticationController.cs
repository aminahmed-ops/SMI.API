using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMI.API.Controllers._Base;
using SMI.AuthService.Interfaces;
using SMI.Entities.DTOs;
using SMI.Util.Response;

namespace SMI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// SIGN IN WITH FACEBOOK
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> FacebookSignIn(FacebookSignInVM model)
        {
            try
            {
                return ReturnResponse(await _authService.TokenAuthenticationWithFacebook(model));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
