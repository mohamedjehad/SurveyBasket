namespace SurveyBasket.Api.Errors;

public class VoteErrors
{
    public static readonly Error InvalidQuestions
        = new("Vote.InvalidQuestions", "Invalid Questions", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedVote
       = new("Vote.Duplicate", "This user had already voted for this poll", StatusCodes.Status409Conflict);
}
