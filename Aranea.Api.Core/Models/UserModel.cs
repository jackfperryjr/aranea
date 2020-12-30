using System;  
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Aranea.Api.Core.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [MaxLength(255)]
        public string Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public string RoleName { get; set; }
        public DateTime JoinDate { get; set; }
        [JsonIgnore]
        public string Token { get; set; }
        public ICollection<PhotoModel> Photos { get; set; }
    }
}
