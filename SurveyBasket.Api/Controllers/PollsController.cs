namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
 private readonly IPollService _pollService= pollService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
    {
      var poll = await _pollService.GetAsync(id);

        return poll is null?NotFound(): Ok(poll.Adapt<PollResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]PollRequest request,CancellationToken cancellationToken)
    {
        var newPoll= await _pollService.Add(request.Adapt<Poll>(),cancellationToken);

        return CreatedAtAction(nameof(Get), new {id = newPoll.Id}, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public async Task <IActionResult> Update([FromRoute] int id,[FromBody] PollRequest request,CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
        if(!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id,cancellationToken);
        if(!isDeleted) return NotFound();
        return NoContent();
    }
    [HttpPut("togglepublish/{id}")]

    public async Task<IActionResult> TogglePublish([FromRoute]int id,CancellationToken cancellationToken)
    {
        var isUpdated= await _pollService.TogglePublishStatusAsync(id,cancellationToken);
        if (!isUpdated) return NotFound();

        return NoContent();
    }

}