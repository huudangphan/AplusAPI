using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Attributes
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var keys = new System.Collections.Generic.List<string>();
            var suffixes = "Model";
            foreach (var key in context.SchemaRepository.Schemas.Keys)
            {
                if (!key.EndsWith(suffixes))
                {
                    keys.Add(key);
                }
            }
            foreach (var key in keys)
            {
                context.SchemaRepository.Schemas.Remove(key);
            }
        }
    }
}
