using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api.Services;
public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context=context;
    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
       var polls = await _context.Polls.AsNoTracking().ToListAsync();

        return polls.Adapt<IEnumerable<PollResponse>>();
    }
        
    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id);

        return poll is null
            ? Result.Failure<PollResponse>(PollErrors.NotFoundPoll)
            : Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> Add(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistingTitle= await _context.Polls.AnyAsync(x=>x.Title == request.Title,cancellationToken);

        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
        

        var poll = request.Adapt<Poll>();
        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _context.Polls.FindAsync(id);
        if (existingPoll is null)
            return Result.Failure(PollErrors.NotFoundPoll);

        var isExistingTitle = await _context.Polls.AnyAsync(x => x.Title == request.Title&& x.Id != id,cancellationToken);

        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);


        existingPoll.Title = request.Title;
        existingPoll.Summary = request.Summary;
        existingPoll.StartsAt = request.StartsAt;
        existingPoll.EndsAt = request.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id);
        if (poll is null)
            return Result.Failure(PollErrors.NotFoundPoll);

        _context.Polls.Remove(poll);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _context.Polls.FindAsync(id);
        if (existingPoll is null)
            return Result.Failure(PollErrors.NotFoundPoll);

        existingPoll.IsPublished = !existingPoll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}