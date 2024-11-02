using SMI.Entities.DTOs;
using SMI.Util.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<JwtResponseVM>> SignInWithFacebook(FacebookSignInVM model);
        Task<BaseResponse<JwtResponseVM>> SignInWithSocialMedia(SocialMediaVM model);
    }
}
