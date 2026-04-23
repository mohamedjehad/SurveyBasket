namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static readonly Error NotFoundQuestion
        = new("Question.NotFound", "No Question Was Found with the given Id", 404);
    public static readonly Error DuplicatedQuestion
       = new("Question.Duplicate", "another Question with the same content exist", 409);
}
