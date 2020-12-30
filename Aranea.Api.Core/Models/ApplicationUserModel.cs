using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Aranea.Api.Core.Models
{
    public class ApplicationUserModel : IdentityUser
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        [MaxLength(255)]
        public string Bio { get; set; }
        public string RoleName { get; set; }
        public DateTime JoinDate { get; set; }
        public string Token { get; set; }
        public string LoggedInIP { get; set; }
        public string LoggedInCity { get; set; }
        public string LoggedInRegion { get; set; }
        public string LoggedInCountry { get; set; }
        public ICollection<PhotoModel> Photos { get; set; }
    }
}
