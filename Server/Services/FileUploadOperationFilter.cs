using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Server.Services 
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadAction = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            
            if (fileUploadAction != null)
            {
                var parameters = fileUploadAction.MethodInfo.GetParameters();
                foreach (var param in parameters)
                {
                    var attribute = param.GetCustomAttributes(typeof(FromFormAttribute), false).FirstOrDefault();
                    if (attribute != null && param.ParameterType == typeof(IFormFile))
                    {
                        operation.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["multipart/form-data"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            [param.Name] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary"
                                            }
                                        },
                                        Required = new HashSet<string> { param.Name }
                                    }
                                }
                            }
                        };
                        operation.Parameters = operation.Parameters.Where(p => p.Name != param.Name).ToList();
                    }
                }
            }
        }
    }
}