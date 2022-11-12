using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplication.Chat;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    private bool RequiredAuthentication(OperationFilterContext context)
    {
        var anonymousScopesFromMethod = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .ToList();
        if (anonymousScopesFromMethod.Any()) return false;

        var requiredScopesFromMethod = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any();

        if (requiredScopesFromMethod) 
            return requiredScopesFromMethod;

        if (context.MethodInfo.ReflectedType == null) 
            return false;

        return context.MethodInfo.ReflectedType
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy)
            .Any();
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiredAuthentication = RequiredAuthentication(context);
        if (!requiredAuthentication) return;

        if (!operation.Responses.ContainsKey("401"))
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        if (!operation.Responses.ContainsKey("403"))
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        var oAuthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };

         operation.Security = new List<OpenApiSecurityRequirement>
         {
             new()
             {
                 [oAuthScheme] = new List<string>()
             }
        };
    }
}