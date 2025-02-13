using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Server.Services
{
    public class ExcludeUploadImageFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // The path should match your route exactly as defined in the controller
            var pathToRemove = "/api/Authentication/upload-image/{userId}";
            swaggerDoc.Paths.Remove(pathToRemove);
        }
    }
}

