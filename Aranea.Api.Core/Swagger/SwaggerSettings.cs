using Microsoft.OpenApi.Models;

namespace Aranea.Api.Core.Swagger
{
    public class SwaggerSettings
    {
        public SwaggerSettings()
        {
            Name = "chocoboAPI";
            Info = new OpenApiInfo
            {
                Title = "chocoboAPI",
                Description = "Authentication API."
            };
        }

        public string Name { get; set; }
        public OpenApiInfo Info { get; set; }
    }
}