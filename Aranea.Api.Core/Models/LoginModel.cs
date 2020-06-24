using System.ComponentModel.DataAnnotations;

namespace Aranea.Api.Core.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Audience { get; set; }
    }
}