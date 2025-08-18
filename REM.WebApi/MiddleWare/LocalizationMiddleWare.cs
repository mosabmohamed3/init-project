using REM.Application.Helper;

namespace REM.WebApi.MiddleWare;

public class LocalizationMiddleWare(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly List<string> _supportedLanguages = ["en", "ar"];

    public async Task Invoke(HttpContext context)
    {
        var headerValue =
            context.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.Value ?? "en";

        var language = _supportedLanguages.Contains(headerValue) ? headerValue : "en";

        CultureHelper.SetLanguage(language);

        await _next(context);
    }
}
