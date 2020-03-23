using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace Aranea.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Age { get; set; }
        public string Picture { get; set; }
        public string Wallpaper { get; set; }
        public string RoleName { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
