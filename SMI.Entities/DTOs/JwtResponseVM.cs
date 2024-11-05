using SMI.Common.Response;

namespace SMI.Entities.DTOs
{
    public class JwtResponseVM : BaseResponse<JwtResponseVM> 
    {
        public string Token { get; set; }
    }
}
