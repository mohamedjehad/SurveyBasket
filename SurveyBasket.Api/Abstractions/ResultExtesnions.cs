namespace SurveyBasket.Api.Abstractions;

public static class ResultExtesnions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success response to problem");

        var problem = Results.Problem(statusCode:result.Error.StatusCode);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                result.Error.Code,new [] {result.Error}
            }
        };

        return new ObjectResult(problemDetails);
    }
}
