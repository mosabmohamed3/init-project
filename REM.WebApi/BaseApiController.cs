using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using REM.Domain.Dto;

namespace REM.WebApi;

[Authorize]
[Route("/api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected ActionResult<Result<T>> BaseResponseHandler<T>(Result<T> response)
    {
        return Ok(response);
    }
}
