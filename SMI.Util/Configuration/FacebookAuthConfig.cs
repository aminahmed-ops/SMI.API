using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.Util.Configuration
{
    public class FacebookAuthConfig
    {
        public string TokenValidationUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
