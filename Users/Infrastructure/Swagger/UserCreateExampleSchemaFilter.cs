using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace movesys_backend_.Users.Infrastructure.Swagger;

public class UserCreateExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Solo aplicar para el tipo User
        if (context.Type.Name == "User" && context.SchemaRepository.Schemas.ContainsKey("User"))
        {
            // Ejemplo en camelCase (formato que devuelve el backend)
            // El backend acepta tanto PascalCase como camelCase en las solicitudes
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["firstName"] = new Microsoft.OpenApi.Any.OpenApiString("Juan"),
                ["lastName"] = new Microsoft.OpenApi.Any.OpenApiString("Pérez"),
                ["dni"] = new Microsoft.OpenApi.Any.OpenApiString("12345678"),
                ["phoneNumber"] = new Microsoft.OpenApi.Any.OpenApiString("987654321"),
                ["role"] = new Microsoft.OpenApi.Any.OpenApiString("driver"),
                ["status"] = new Microsoft.OpenApi.Any.OpenApiString("active")
            };
        }
    }
}

