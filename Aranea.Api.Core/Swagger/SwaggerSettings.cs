using Microsoft.OpenApi.Models;

namespace Aranea.Api.Core.Swagger
{
    public class SwaggerSettings
    {
        public SwaggerSettings()
        {
            Name = "ChocoboApi";
            Info = new OpenApiInfo
            {
                Title = "ChocoboApi",
                Description = "Authentication API."
            };
        }

        public string Name { get; set; }
        public OpenApiInfo Info { get; set; }
    }
}