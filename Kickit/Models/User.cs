using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kickit.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "Your email address is required to sign up.")]
        [MaxLength(320)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your first name is required to sign up.")]
        [MaxLength(30)]
        public  string First_Name { get; set; }

        [Required(ErrorMessage = "Your last name is required to sign up.")]
        [MaxLength(30)]
        public string Last_Name { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "You must set a password to sign up.")]
        [StringLength(70, MinimumLength = 8, ErrorMessage = "Your password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ResetPasswordCode { get; set; }
    }
}