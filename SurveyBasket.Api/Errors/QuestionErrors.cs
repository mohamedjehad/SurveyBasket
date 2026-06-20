namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static readonly Error NotFoundQuestion
        = new("Question.NotFound", "No Question Was Found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestion
       = new("Question.Duplicate", "another Question with the same content exist", StatusCodes.Status409Conflict);
}
