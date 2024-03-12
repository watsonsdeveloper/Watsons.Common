using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Call the next middleware in the pipeline
            await _next(context);

            if (context.Items["ValidationErrors"] is List<ValidationResult> errors && errors.Any())
            {
                var problemDetails = new ValidationProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request validation error.",
                    Detail = "See errors for details.",
                    Instance = context.Request.Path
                };

                problemDetails.Errors.Add("ValidationErrors", errors.Select(e => e.ErrorMessage).ToArray());

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                //await context.Response.WriteAsJsonAsync(problemDetails);
                //await context.Response.WriteAsync(problemJson);
                var problemJson = System.Text.Json.JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(problemJson);
            }

        }
    }

}
