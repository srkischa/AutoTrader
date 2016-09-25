using System.ComponentModel.DataAnnotations;

namespace AutoTrader.WebApi.Request
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}