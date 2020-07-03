using System;
using System.ComponentModel.DataAnnotations;

namespace Aranea.Api.Core.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "email is not a valid email address")]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        public DateTime JoinDate { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "your password must be between {2} and {1} characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "the passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Audience { get; set; }
    }
}