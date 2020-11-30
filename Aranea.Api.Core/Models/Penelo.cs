using System.Collections.Generic;

namespace Aranea.Api.Core.Models
{
    public class Penelo
    {
        public int count { get; set; }
        public List<Room> rooms { get; set; }
    }

    public class Room
    {
        public string name { get; set; }
        public string created_at { get; set; }
        public string destroy_url { get; set; }
    }
}