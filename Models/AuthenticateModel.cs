using System.ComponentModel.DataAnnotations;

namespace Aranea.Models
{
    public class AuthenticateModel
    {
        public string Id { get; set; }

        public string Token { get; set; }
    }
}