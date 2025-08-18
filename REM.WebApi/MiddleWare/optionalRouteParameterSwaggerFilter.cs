using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace REM.WebApi.MiddleWare;

public class AddRequiredHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "Accept-Language",
                Required = false,
                In = ParameterLocation.Header,
                Description = "Localization Header",
                Schema = new OpenApiSchema { Type = "string" },
            }
        );
    }
}

// Convert get request parameters to query parameters in swagger
public class FromQueryModelFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var description = context.ApiDescription;
        if (description.HttpMethod!.ToLower() != HttpMethod.Get.ToString().ToLower())
        {
            // We only want to do this for GET requests, if this is not a
            // GET request, leave this operation as is, do not modify
            return;
        }

        var actionParameters = description.ActionDescriptor.Parameters;
        var apiParameters = description
            .ParameterDescriptions.Where(p => p.Source.IsFromRequest)
            .ToList();

        if (actionParameters.Count == apiParameters.Count)
        {
            // If no complex query parameters detected, leave this operation as is, do not modify
            return;
        }

        operation.Parameters = CreateParameters(actionParameters, operation.Parameters, context);
    }

    private IList<OpenApiParameter> CreateParameters(
        IList<ParameterDescriptor> actionParameters,
        IList<OpenApiParameter> operationParameters,
        OperationFilterContext context
    )
    {
        var newParameters = actionParameters
            .Select(p => CreateParameter(p, operationParameters, context))
            .Where(p => p != null) // Filter out nulls
            .Select(p => p!) // Use null-forgiving operator
            .ToList();

        return newParameters.Any() ? newParameters : [];
    }

    private OpenApiParameter? CreateParameter(
        ParameterDescriptor actionParameter,
        IList<OpenApiParameter> operationParameters,
        OperationFilterContext context
    )
    {
        var operationParamNames = operationParameters.Select(p => p.Name);
        if (operationParamNames.Contains(actionParameter.Name))
        {
            // If param is defined as the action method argument, just pass it through
            return operationParameters.First(p => p.Name == actionParameter.Name);
        }

        if (actionParameter.BindingInfo == null)
        {
            return null;
        }

        var generatedSchema = context.SchemaGenerator.GenerateSchema(
            actionParameter.ParameterType,
            context.SchemaRepository
        );

        var newParameter = new OpenApiParameter
        {
            Name = actionParameter.Name,
            In = ParameterLocation.Query,
            Schema = generatedSchema,
        };

        return newParameter;
    }
}

public class NonNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var property in schema.Properties)
        {
            property.Value.Nullable = false;
        }
    }
}

public class RequiredPropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var requiredProperties = context
            .Type.GetProperties()
            .Where(prop =>
                prop.IsDefined(
                    typeof(System.Runtime.CompilerServices.RequiredMemberAttribute),
                    true
                )
            )
            .Select(prop => prop.Name);

        foreach (var propName in requiredProperties)
        {
            var propLower = char.ToLowerInvariant(propName[0]) + propName.Substring(1);
            ;
            if (schema.Properties.ContainsKey(propLower))
            {
                var propertySchema = schema.Properties[propLower];

                // Mark the property as required in the OpenAPI schema
                if (!schema.Required.Contains(propLower))
                {
                    schema.Required.Add(propLower);
                }

                // Ensure that nullable is set to false
                propertySchema.Nullable = false;
            }
        }
    }
}
