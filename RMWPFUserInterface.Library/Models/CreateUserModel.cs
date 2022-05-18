using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        //[RegularExpression("")] // TODO: Create a regular expression to check the password
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
