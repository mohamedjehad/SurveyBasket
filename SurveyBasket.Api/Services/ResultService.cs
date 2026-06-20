using SurveyBasket.Api.Contracts.Results;

namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var poll = _context.Polls.Where(x => x.Id == pollId && x.IsPublished && x.StartsAt <= today && x.EndsAt >= today);
        if (poll is null)
            return Result.Failure<PollVotesResponse>(PollErrors.NotFoundPoll);

        var pollVotes = await _context.Polls.Where(p => p.Id == pollId)
             .Select(x => new PollVotesResponse(x.Title,
             x.Votes.Select(v => new VoteResponse($"{v.User.FirstName} {v.User.LastName}",
             v.SubmittedOn,
             v.VoteAnswers.Select(x => new QuestionAnswerResponse(x.Question.Content, x.Answer.Content
                    ))
                 ))
            ))
             .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is null
        ? Result.Failure<PollVotesResponse>(PollErrors.NotFoundPoll)
        : Result.Success(pollVotes);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId,CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var poll = _context.Polls.Where(x => x.Id == pollId && x.IsPublished && x.StartsAt <= today && x.EndsAt >= today);
        if (poll is null)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.NotFoundPoll);

        var votes=await _context.Votes
            .Where(x=>x.PollId==pollId)
            .GroupBy(x=> new {Date=DateOnly.FromDateTime(x.SubmittedOn)})
            .Select(g=>new VotesPerDayResponse
            (g.Key.Date,
            g.Count()))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votes);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var poll = _context.Polls.Where(x => x.Id == pollId && x.IsPublished && x.StartsAt <= today && x.EndsAt >= today);
        if (poll is null)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.NotFoundPoll);

        var votesPerQuestion = await _context.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .Select(x => new VotesPerQuestionResponse(
                x.Question.Content,
                x.Question.Votes
                .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                .Select(g => new VotesPerAnswerResponse(g.Key.AnswerContent, g.Count()))
                )).ToListAsync(cancellationToken);


        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}
