using IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMI.AuthService.Interfaces;
using SMI.AuthService.Interfaces.Facebook;
using SMI.Entities.DTOs;
using SMI.Entities.Entities;
using SMI.Util.Configuration;
using SMI.Util.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SMI.AuthService.Services
{
    /// <summary>
    /// Class Auth Service.
    /// Implements the <see cref="SocialAuthentication.Interfaces.IAuthService" />
    /// </summary>
    /// <seealso cref="SocialAuthentication.Interfaces.IAuthService" />
    public class AuthService : IAuthService
    {
        //private readonly ApplicationDbContext _context;
        //private readonly IGoogleAuthService _googleAuthService;
        private readonly IFacebookAuthService _facebookAuthService;
        //private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;

        public AuthService(
            IFacebookAuthService facebookAuthService,
          
            IOptions<Jwt> jwt)
        {
            _facebookAuthService = facebookAuthService;
            _jwt = jwt.Value;
        }


        /// <summary>
        /// Facebook SignIn
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtResponseVM>> TokenAuthenticationWithFacebook(FacebookSignInVM model)
        {
            var validatedFbToken = await _facebookAuthService.ValidateFacebookToken(model.AccessToken);

            if (validatedFbToken.Errors.Any())
                return new BaseResponse<JwtResponseVM>(validatedFbToken.ResponseMessage, validatedFbToken.Errors);

            var userInfo = await _facebookAuthService.GetFacebookUserInformation(model.AccessToken);

            if (userInfo.Errors.Any())
                return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

            var userToBeCreated = new CreateUserFromSocialLogin
            {
                FirstName = userInfo.Data.FirstName,
                LastName = userInfo.Data.LastName,
                Email = userInfo.Data.Email,
                ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
                LoginProviderSubject = userInfo.Data.Id,
            };

           // var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Facebook);

            //if (user is not null)
            //{
            //    var jwtResponse = CreateJwtToken(user);

            //    var data = new JwtResponseVM
            //    {
            //        Token = jwtResponse,
            //    };

            //    return new BaseResponse<JwtResponseVM>(data);
            //}

            return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

        }

        /// <summary>
        /// Creates JWT Token
        /// </summary>
        /// <param name="user">the user</param>
        /// <returns>System.String</returns>
        private string CreateJwtToken(User user)
        {

            var key = Encoding.ASCII.GetBytes(_jwt.Secret);

            var userClaims = BuildUserClaims(user);

            var signKey = new SymmetricSecurityKey(key);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.ValidIssuer,
                notBefore: DateTime.UtcNow,
                audience: _jwt.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwt.DurationInMinutes)),
                claims: userClaims,
                signingCredentials: new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// Builds the UserClaims
        /// </summary>
        /// <param name="user">the User</param>
        /// <returns>List&lt;System.Security.Claims&gt;</returns>
        private List<Claim> BuildUserClaims(User user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return userClaims;
        }

    }
}
