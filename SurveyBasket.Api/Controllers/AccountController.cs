using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Authentication.Filters;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Controllers;

[Route("v:{v:apiVersion}/me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> Info()
    {
        var result =await _userService.GetProfileAsync(User.GetUserId()!);
        return Ok(result.Value);
    }

    [HttpPut("info")]
    [HasPermission(Permissions.UpdateUsers)]

    public async Task<IActionResult> Info([FromBody]UpdateProfileRequest request)
    {
        var result = await _userService.UpdateProfileAsync(request,User.GetUserId()!);
        return NoContent();
    }

    [HttpPut("change-password")]
    [HasPermission(Permissions.UpdateUsers)]

    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(request, User.GetUserId()!);
        return result.IsSuccess ?
            NoContent()
            : result.ToProblem();
    }

}
