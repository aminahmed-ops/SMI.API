using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.Common.Configuration
{
    public class BaseClient
    {
        public virtual string AppId { get; set; }
        public virtual string AppSecret { get; set; }
        public virtual string TokenValidationUrl { get; set; }
        public virtual string UserInfoUrl { get; set; }
        public virtual string BaseUrl { get; set; }
    }
}
