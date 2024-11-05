
using SMI.Entities.DTOs;

namespace SMI.BusinessLogic.Areas.Authentication.Services
{
    public interface IAuthService
    {
        Task<JwtResponseVM> SignInWithSocialMedia(SocialMediaVM model);
    }
}
