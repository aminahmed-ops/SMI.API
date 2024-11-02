using SMI.Util.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.Entities.DTOs
{
    public class SocialMediaVM
    {
        /// <summary>
        /// This token is generated from the client side. i.e. react, angular, flutter etc.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public LoginProvider LoginProvider { get; set; }
    }
}
