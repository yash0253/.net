using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Exceptions;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EnterpriseServiceManagement.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(
     ex,
     "Unhandled exception occurred. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
     context.Request.Method,
     context.Request.Path,
     context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,

            ConflictException => HttpStatusCode.Conflict,

            _ => HttpStatusCode.InternalServerError
        };
        //_ means "anything else" or "any other case".
        //        Is it ArgumentException ?
        //        ↓ No
        //Is it UnauthorizedAccessException?
        //        ↓ No
        //Is it KeyNotFoundException?
        //        ↓ No
        //Anything else?
        //        ↓ Yes
        //HTTP 500 Internal Server Error

        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponseDto
        {
            Success = false,
            Message = exception switch
            {
                NotFoundException => exception.Message,

                ConflictException => exception.Message,

                _ => "An unexpected error occurred."
            },
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}