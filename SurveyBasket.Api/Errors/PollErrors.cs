namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error NotFoundPoll
        = new("Poll.NotFound", "No Poll Was Found with the given Id",StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedPollTitle
       = new("Poll.Duplicate", "another Poll with the same title exist", StatusCodes.Status409Conflict);
}
