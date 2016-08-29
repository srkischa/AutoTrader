using System.ComponentModel.DataAnnotations;

namespace AutoTrader.WebApi.Request
{
    public class UserRegistrationRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}