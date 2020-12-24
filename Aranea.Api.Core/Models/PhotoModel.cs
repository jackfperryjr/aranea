using System;  
using System.ComponentModel.DataAnnotations;  
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Aranea.Api.Core.Models  
{  
    public class PhotoModel
    {  
        public Guid Id { get; set; }  
        public string Url { get; set; }
        public int Portrait { get; set; } = 0;
        public int Wallpaper { get; set; } = 0;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUserModel User { get; set; }
    }  
}