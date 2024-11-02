using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMI.AuthService.Interfaces;
using SMI.AuthService.Interfaces.Facebook;
using SMI.DataAccess.Context;
using SMI.Entities.DTOs;
using SMI.Entities.Entities;
using SMI.Util.Configuration;
using SMI.Util.Enum;
using SMI.Util.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMI.AuthService.Services
{
    /// <summary>
    /// Class Auth Service.
    /// Implements the <see cref="SMI.AuthService.Services.IAuthService" />
    /// </summary>
    /// <seealso cref="SMI.AuthService.Services.IAuthService" />
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        //private readonly IGoogleAuthService _googleAuthService;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;

        public AuthService(
            ApplicationDbContext context,
            IFacebookAuthService facebookAuthService,
           UserManager<User> userManager,
            IOptions<Jwt> jwt)
        {
            _context = context;
            _facebookAuthService = facebookAuthService;
            _userManager = userManager;
            _jwt = jwt.Value;
        }


        /// <summary>
        /// Facebook SignIn
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtResponseVM>> SignInWithFacebook(FacebookSignInVM model)
        {
            var validatedFbToken = await _facebookAuthService.ValidateFacebookToken(model.AccessToken);

            if (validatedFbToken.Errors.Any())
                return new BaseResponse<JwtResponseVM>(validatedFbToken.ResponseMessage, validatedFbToken.Errors);

            var userInfo = await _facebookAuthService.GetFacebookUserInformation(model.AccessToken);

            if (userInfo.Errors.Any())
                return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

            //var userToBeCreated = new CreateUserFromSocialLogin
            //{
            //    FirstName = userInfo.Data.FirstName,
            //    LastName = userInfo.Data.LastName,
            //    Email = userInfo.Data.Email,
            //    ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
            //    LoginProviderSubject = userInfo.Data.Id,
            //};
            //userToBeCreated.Email = "cap.kumail@gmail.com";
            ////var abc = LoginProvider.Facebook.ToString().ToUpper();
            //var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Facebook);

            var user = new User
            {
                FirstName = userInfo.Data.FirstName,
                LastName = userInfo.Data.LastName,
                Email = userInfo.Data.Email,
                ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
            
            };
            user.Email = "cap.kumail@gmail.com";

            if (user is not null)
            {
                var jwtResponse = CreateJwtToken(user);

                var data = new JwtResponseVM
                {
                    Token = jwtResponse,
                };

                return new BaseResponse<JwtResponseVM>(data);
            }

            return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

        }


        /// <summary>
        /// Facebook SignIn
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtResponseVM>> SignInWithSocialMedia(SocialMediaVM model)
        {
            if (model != null)
            {
                if (model.LoginProvider == LoginProvider.Facebook)
                {
                    var validatedFbToken = await _facebookAuthService.ValidateFacebookToken(model.AccessToken);

                    if (validatedFbToken.Errors.Any())
                        return new BaseResponse<JwtResponseVM>(validatedFbToken.ResponseMessage, validatedFbToken.Errors);

                    var userInfo = await _facebookAuthService.GetFacebookUserInformation(model.AccessToken);

                    if (userInfo.Errors.Any())
                        return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

                    //var userToBeCreated = new CreateUserFromSocialLogin
                    //{
                    //    FirstName = userInfo.Data.FirstName,
                    //    LastName = userInfo.Data.LastName,
                    //    Email = userInfo.Data.Email,
                    //    ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
                    //    LoginProviderSubject = userInfo.Data.Id,
                    //};
                    //userToBeCreated.Email = "cap.kumail@gmail.com";
                    //var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Facebook);
                    var user = new User
                    {
                        FirstName = userInfo.Data.FirstName,
                        LastName = userInfo.Data.LastName,
                        Email = userInfo.Data.Email,
                        ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
                        Id = 1
                    };
                    if (user is not null)
                    {
                        var jwtResponse = CreateJwtToken(user);

                        var data = new JwtResponseVM
                        {
                            Token = jwtResponse,
                        };

                        return new BaseResponse<JwtResponseVM>(data);
                    }

                    return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);
                }
            }
            return new BaseResponse<JwtResponseVM>(null, "Error");


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
