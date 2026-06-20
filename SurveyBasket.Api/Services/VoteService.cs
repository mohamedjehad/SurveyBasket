using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId,string userId, VoteRequest request, CancellationToken cancellationToken=default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
       var pollIsExist=await _context.Polls.AnyAsync(p=>p.Id==pollId&&p.IsPublished&&p.StartsAt<=today&&p.EndsAt>=today);

        if (!pollIsExist)
            return Result.Failure(PollErrors.NotFoundPoll);

        var hasVotes=await _context.Votes.AnyAsync(x=>x.PollId==pollId&&x.UserId==userId);

        if (hasVotes)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var availableQuestions=await _context.Questions
            .Where(x=>x.PollId==pollId&&x.IsActive)
            .Select(x=>x.Id).ToListAsync();

        var isEqual=request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions);

        if (!isEqual)
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
