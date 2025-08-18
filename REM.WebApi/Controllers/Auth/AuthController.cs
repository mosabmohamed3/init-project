// using MediatR;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using REM.Application.Services.Abstraction.TokenService;
// using REM.Domain.Dto;


// namespace REM.WebApi.Controllers.Auth;

// [AllowAnonymous]
// public class AuthController(ISender sender) : BaseApiController
// {
//     private readonly ISender _mediator = sender;

//     [HttpPost("loginAdmin")]
//     public async Task<ActionResult<Result<TokenDto>>> LoginAdmin(LoginAdminCommand command)
//         => BaseResponseHandler(await _mediator.Send(command));

//     [HttpPost("loginUser")]
//     public async Task<ActionResult<Result<TokenDto>>> LoginUser(LoginUserCommand command)
//         => BaseResponseHandler(await _mediator.Send(command));

//     [HttpPost("refreshToken")]
//     public async Task<ActionResult<Result<TokenDto>>> RefreshToken(RefreshTokenCommand command)
//     {
//         var response = await _mediator.Send(command);
//         if (response.Succeeded == false)
//             return Unauthorized();
//         return BaseResponseHandler(response);
//     }

//     [HttpPost("register")]
//     public async Task<ActionResult<Result<RegisterCommandResponse>>> Register(RegistrationCommand command)
//         => BaseResponseHandler(await _mediator.Send(command));

//     [Authorize]
//     [HttpDelete("logout")]
//     public async Task<ActionResult<Result<Unit>>> Logout(LogoutCommand command)
//         => BaseResponseHandler(await _mediator.Send(command));
// }
