namespace SurveyBasket.Api.Services;
public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context=context;
    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Polls.AsNoTracking().ToListAsync();
        
    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default) =>
       await _context.Polls.FindAsync(id);

    public async Task<Poll> Add(Poll poll,CancellationToken cancellationToken=default)
    {
        await _context.AddAsync(poll,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken=default)
    {
        var existingPoll= await GetAsync(id);
        if (existingPoll is null)
            return false;

        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id,CancellationToken cancellationToken=default)
    {
        var poll = await GetAsync(id);
        if (poll is null) return false;

        _context.Polls.Remove(poll);

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll= await GetAsync(id);
        if (existingPoll is null) return false;

        existingPoll.IsPublished = !existingPoll.IsPublished;

        await _context.SaveChangesAsync (cancellationToken);
        return true;
    }
}