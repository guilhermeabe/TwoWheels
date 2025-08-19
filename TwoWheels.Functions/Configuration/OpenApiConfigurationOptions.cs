using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace TwoWheels.Functions.Configuration
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "1.0.0",
            Title = "TwoWheels API",
            Description = "API para gerenciamento de motos, entregadores e locações",
            Contact = new OpenApiContact()
            {
                Name = "TwoWheels",
                Email = "teste@twowheels.com",
                Url = null
            },
            License = null
        };
    }
}