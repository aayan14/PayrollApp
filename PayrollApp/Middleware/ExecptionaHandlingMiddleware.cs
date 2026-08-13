using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace PayrollApp.Middleware
{
    public class ExecptionaHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExecptionaHandlingMiddleware> _logger;

        public ExecptionaHandlingMiddleware(RequestDelegate next, ILogger<ExecptionaHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                _logger.LogError("Unexpected error has occured at Path : {Path}", context.Request.Path);
                await HandleExecptionAsync(context, ex);
            }
        }

        private static async Task HandleExecptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statuscode, message) = ex switch
            {
                SqlException e when e.Message.Contains("already exists")
        => (HttpStatusCode.Conflict, e.Message),
                InvalidOperationException e when e.Message.Contains("already exists")
                => (HttpStatusCode.Conflict, e.Message),
                InvalidOperationException e
                => (HttpStatusCode.BadRequest, e.Message),
                KeyNotFoundException e
                => (HttpStatusCode.NotFound, e.Message),
                ArgumentException e
                => (HttpStatusCode.BadRequest, e.Message),
                _ 
                => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = (int)statuscode;

            var response = new
            {
                statuscode = statuscode,
                message,
                path = context.Request.Path.ToString()
            };

            

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
