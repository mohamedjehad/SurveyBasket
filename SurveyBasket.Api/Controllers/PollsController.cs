using Mapster;
using SurveyBasket.Api.Contracts.Requests;
using SurveyBasket.Api.Contracts.Responses;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
 private readonly IPollService _pollService= pollService;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll().Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute]int id)
    {
      var poll = _pollService.Get(id);

        return poll is null?NotFound(): Ok(poll.Adapt<PollResponse>());
    }

    [HttpPost]
    public IActionResult Add([FromBody]CreatePollRequest request)
    {
        var newPoll= _pollService.Add(request.Adapt<Poll>());

        return CreatedAtAction(nameof(Get), new {id = newPoll.Id}, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id,[FromBody] CreatePollRequest request)
    {
     return _pollService.Update(id,request.Adapt<Poll>()) ? NoContent() : NotFound();      
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        return _pollService.Delete(id) ? NoContent() : NotFound();
    }

}