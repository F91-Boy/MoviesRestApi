using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _prodiver;
        private readonly IHostEnvironment _environment;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider prodiver, IHostEnvironment environment)
        {
            _prodiver = prodiver;
            _environment = environment;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _prodiver.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = _environment.ApplicationName,
                        Version = description.ApiVersion.ToString(),
                    });
            }

            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme 
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Please provide a valid token(请提供一个有效的token)",
                Name = "Authorization",
                Type=Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                BearerFormat ="JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()

                }
            }); 

        }
    }
}
