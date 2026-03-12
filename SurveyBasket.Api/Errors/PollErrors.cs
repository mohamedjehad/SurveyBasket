namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error NotFoundPoll
        = new("Poll.NotFound", "No Poll Was Found with the given Id",404);
    public static readonly Error DuplicatedPollTitle
       = new("Poll.Duplicate", "another Poll with the same title exist", 409);
}
