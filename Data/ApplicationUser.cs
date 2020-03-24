using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;

namespace Aranea.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Age { get; set; } = 0;
        public string Photo { get; set; }
        public string Wallpaper { get; set; }
        [MaxLength(255)]
        public string Profile { get; set; }
        public string RoleName { get; set; }
        public DateTime JoinDate { get; set; }
        public string Token { get; set; }
        public string LoggedInIP { get; set; }
        public string LoggedInCity { get; set; }
        public string LoggedInRegion { get; set; }
        public string LoggedInCountry { get; set; }
    }
}
