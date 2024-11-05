using Microsoft.AspNetCore.Mvc;
using SMI.API.Controllers._Base; 
using SMI.Entities.DTOs;
using SMI.Common.Response;
using SMI.BusinessLogic.Areas.Authentication.Services;

namespace SMI.API.Areas.Authentication
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// SIGN IN WITH SOCIAL MEDIA
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> SocialMediaSignIn(SocialMediaVM model)
        {
            try
            {
                return ReturnResponse(await _authService.SignInWithSocialMedia(model));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

    }
}
