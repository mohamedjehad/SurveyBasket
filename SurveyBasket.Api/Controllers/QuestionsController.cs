
using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Authentication.Filters;
using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Questions;
using System.Reflection.Metadata.Ecma335;

namespace SurveyBasket.Api.Controllers;

[Route("api/v:{v:apiVersion}/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet]
    [HasPermission(Permissions.GetQuestions)]

    public async Task<IActionResult> GetAll([FromRoute]int pollId, [FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var questions = await _questionService.GetAllAsync(pollId,filters, cancellationToken);
        return Ok(questions);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetQuestions)]

    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var question = await  _questionService.GetAsync(pollId,id,cancellationToken);
        return question is null
            ? NotFound()
            : Ok(question);
    }

    [HttpPost]
    [HasPermission(Permissions.AddQuestions)]

    public async Task<IActionResult> Add([FromRoute]int pollId,[FromBody]QuestionRequest request,CancellationToken cancellationToken)
    {
        var result = await _questionService.AddAsync(pollId, request, cancellationToken);
        return result.IsSuccess?
            CreatedAtAction(nameof(Get),new {pollId,result.Value.Id},result.Value)
            :result.ToProblem();
    }
    [HttpPut("{id}/togglestatus")]
    [HasPermission(Permissions.UpdateQuestions)]

    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
        return result.IsSuccess ?
            NoContent()
            :result.ToProblem();
    }
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> Update([FromRoute]int pollId,[FromRoute]int id,[FromBody]QuestionRequest request,CancellationToken cancellationToken)
    {
        var result= await _questionService.UpdateAsync(pollId,id,request,cancellationToken);
        return result.IsSuccess?
            NoContent()
            :result.ToProblem();
    }
}
