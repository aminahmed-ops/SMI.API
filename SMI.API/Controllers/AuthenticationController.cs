using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SMI.API.Controllers._Base;
using SMI.AuthService.Interfaces;
using SMI.Entities.DTOs;
using SMI.Entities.Entities;
using SMI.Util.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;

namespace SMI.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<User> _signInManager;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
           // _signInManager = signInManager;
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
                return ReturnResponse(await _authService.SignInWithFacebook(model));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
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
        [AllowAnonymous]
        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback([FromQuery] string state = null)
        {
            // Handle the authentication result from Facebook
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return Unauthorized();

            // Extract user claims or access token as needed
            var claims = result.Principal?.Identities
                          .FirstOrDefault()?.Claims.Select(claim => new { claim.Type, claim.Value });

            return Ok(claims);
        }
        [HttpGet("login-facebook")]
        public IActionResult LoginWithFacebook()
        {
            var redirectUrl = Url.Action("FacebookCallback", "Authentication");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
    }
}
