using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    /// <summary>
    /// Information needed to send in order receive an access token from the server.
    /// </summary>
    public class AuthenticationUserModel
    {
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
