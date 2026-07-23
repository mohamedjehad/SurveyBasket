using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Authentication.Filters;
using System.Runtime.CompilerServices;

namespace SurveyBasket.Api.Controllers;

[Route("api/v:{v:apiVersion}/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [HasPermission(Permissions.GetUsers)]
    
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetUsers)]   
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var result = await _userService.GetAsync(id);
        return result.IsSuccess ?
            Ok(result.Value) 
            : result.ToProblem();
    }

    [HttpPost()]
    [HasPermission(Permissions.AddUsers)]   
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return result.IsSuccess ?
            CreatedAtAction(nameof(Get),new { result.Value!.Id },result.Value) 
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateUsers)]   
    public async Task<IActionResult> Update([FromRoute]string id,[FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id,request);
        return result.IsSuccess ?
            NoContent() 
            : result.ToProblem();
    }

    [HttpPut("{id}/toggle-status")]
    [HasPermission(Permissions.UpdateUsers)]   
    public async Task<IActionResult> ToggleStatus([FromRoute]string id)
    {
        var result = await _userService.ToggleStatusAsync(id);
        return result.IsSuccess ?
            NoContent() 
            : result.ToProblem();
    }


    [HttpPut("{id}/unlock")]
    [HasPermission(Permissions.UpdateUsers)]   
    public async Task<IActionResult> Unlock([FromRoute]string id)
    {
        var result = await _userService.UnlockUser(id);
        return result.IsSuccess ?
            NoContent() 
            : result.ToProblem();
    }
}
