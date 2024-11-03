using SMI.Util.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMI.Entities.DTOs.FacebookAuthentication;
namespace SMI.AuthService.Interfaces.Facebook
{
    public interface IFacebookAuthService
    {
        Task<BaseResponse<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
        Task<BaseResponse<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
    }
}
