using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.Entities.DTOs
{
    public class FacebookSignInVM
    {
        /// <summary>
        /// This token is generated from the client side. i.e. react, angular, flutter etc.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }
    }
}
