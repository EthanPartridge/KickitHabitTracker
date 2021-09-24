using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kickit.Models
{
    public class ForgotPasswordViewModel
    {
        [Required (ErrorMessage = "Your email address is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
