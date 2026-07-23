using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize(DefaultRoles.Member)]
[EnableRateLimiting("concurrency")]
public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet]
    public async Task<IActionResult> Start(int pollId,CancellationToken cancellationToken)
    {
        var userId= User.GetUserId();
        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);
        return result.IsSuccess ?
            Ok(result.Value) :
            result.ToProblem();
    }
    [HttpPost]
    public async Task<IActionResult> Add([FromRoute]int pollId,[FromBody] VoteRequest request, CancellationToken cancellationToken = default)
    {
        var userId=User.GetUserId()!;
        var result= await _voteService.AddAsync(pollId,userId,request, cancellationToken);

        return result.IsSuccess?
            Created()
            :result.ToProblem();
    }
}
