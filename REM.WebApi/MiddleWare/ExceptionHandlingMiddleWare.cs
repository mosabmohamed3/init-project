using System.Text;
using REM.Application.Exceptions;
using REM.Application.Resources.Static;
using REM.Domain.Dto;

namespace REM.WebApi.MiddleWare;

public class ExceptionHandlerMiddleWare(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleWare> logger
)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(Result<object>.Fail([.. ex.Errors]));
        }
        catch (Exception ex)
        {
            var request = context.Request;
            var user = context.User.Identity?.Name ?? "Anonymous";
            var requestBody = await ReadRequestBodyAsync(request);

            logger.LogError(ex,
                "\n\n============================================================" +
                "\n⚠️  ERROR OCCURRED DURING REQUEST ⚠️" +
                "\n------------------------------------------------------------" +
                "\n📌 Path       : {Path}" +
                "\n🔹 Method     : {Method}" +
                "\n👤 User       : {User}" +
                "\n📩 Headers    : {Headers}" +
                "\n📝 Body       : {Body}" +
                "\n🆔 Exception  : {ExceptionType}" +
                "\n❌ Message    : {Message}" +
                "\n📜 StackTrace :\n{StackTrace}" +
                "\n============================================================\n",
                request.Path, request.Method, user,
                request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),  // Convert headers to key-value dictionary
                requestBody,
                ex.GetType().Name, ex.Message, ex.StackTrace
            );

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(Result<object>.Fail(ex.ToString()));
        }
    }

    /// <summary>
    /// Reads and returns the request body as a string (if applicable).
    /// </summary>
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            if (request.ContentLength == null || request.ContentLength == 0)
                return "(empty)";

            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
        catch (Exception ex)
        {
            return $"(failed to read body: {ex.Message})";
        }
    }
}
