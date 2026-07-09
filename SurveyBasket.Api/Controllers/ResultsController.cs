using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Authentication.Filters;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[HasPermission(Permissions.GetResults)]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet("row-data")]
    public async Task<IActionResult> PollVotes([FromRoute]int pollId,CancellationToken cancellationToken=default)
    {
        var result = await _resultService.GetPollVotesAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) :
            result.ToProblem();
    }

    [HttpGet("votes-per-day")]
    public async Task<IActionResult> PollVotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken = default)
    {
        var result = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) :
            result.ToProblem();
    }

    [HttpGet("votes-per-question")]
    public async Task<IActionResult> PollVotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken = default)
    {
        var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) :
            result.ToProblem();
    }
}
