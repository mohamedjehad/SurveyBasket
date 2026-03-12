namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]

public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService= pollService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id);

        return result.IsSuccess ?
            Ok(result.Value)
            :result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]PollRequest request,CancellationToken cancellationToken)
    {
        var result = await _pollService.Add(request, cancellationToken);

        return result.IsSuccess?
             CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
             : result.ToProblem();

    }

    [HttpPut("{id}")]
    public async Task <IActionResult> Update([FromRoute] int id,[FromBody] PollRequest request,CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ?
            NoContent()
            : result.ToProblem();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ?
            NoContent()
            : result.ToProblem();
    }
    [HttpPut("togglepublish/{id}")]
    public async Task<IActionResult> TogglePublish([FromRoute]int id,CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess ?
            NoContent()
            : result.ToProblem();
    }

   
}