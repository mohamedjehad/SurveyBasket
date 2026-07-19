using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Polls.Answers;
using SurveyBasket.Api.Contracts.Questions;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context,HybridCache hybridCache,ILogger<QuestionService> logger) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly ILogger _logger = logger;
    private const string _cachePrefix="availableQuestions";

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var pollIsExisted = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
        if (!pollIsExisted)
            return Result.Failure<QuestionResponse>(PollErrors.NotFoundPoll);

        var questionIsExisted=await _context.Questions.AnyAsync(x=>x.Content == request.Content&&pollId==x.PollId, cancellationToken);

        if (questionIsExisted)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestion);

        var question = request.Adapt<Question>();
        question.PollId=pollId;

        await _context.Questions.AddAsync(question,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,RequestFilters filters, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId);
        if (!pollIsExist)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.NotFoundPoll);

        var query = _context.Questions
       .Where(x => x.PollId == pollId);
       
       if(!string.IsNullOrEmpty(filters.SearchValue))
        {
            query = query.Where(x => x.Content.Contains(filters.SearchValue));
        }

       if(!string.IsNullOrEmpty(filters.SortColumn))
        {
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
        } 
        
       var source = query
                       .Include(x => x.Answers)
                       .ProjectToType<QuestionResponse>()
                       .AsNoTracking();
  
        var questions = await PaginatedList<QuestionResponse>.CreateAsync(source,filters.PageSize,filters.PageNumber,cancellationToken);

        return Result.Success(questions);
  }



    public async Task<QuestionResponse?> GetAsync(int pollId,int id,CancellationToken cancellationToken)
    {
        return await _context.Questions
            .Where(x => x.Id == id&&x.PollId==pollId)
            .Include (q=>q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"{_cachePrefix}-{pollId}";
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var pollIsExists = await _context.Polls
            .AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= today && x.EndsAt >= today);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.NotFoundPoll);
        var hasVotes = await _context.Votes.AnyAsync(x => x.UserId == userId && x.PollId == pollId);

        if (hasVotes)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);


        var questions = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async token =>
            {
                return await _context.Questions
                    .Where(q => q.PollId == pollId && q.IsActive)
                    .Include(q => q.Answers)
                    .AsNoTracking()
                    //.Select(q => new QuestionResponse(
                    //    q.Id,
                    //    q.Content,
                    //    q.Answers
                    //        .Where(a => a.IsActive)
                    //        .Select(a => new AnswerResponse(a.Id, a.Content))
                    //))
                    .ProjectToType<QuestionResponse>()
                    .ToListAsync(token);
            },
            cancellationToken: cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
                            .SingleOrDefaultAsync(x => x.Id == questionId && x.PollId == pollId,cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.NotFoundQuestion);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken)
    {
        var questionIsExisted= await _context.Questions
            .AnyAsync(x=>x.PollId== pollId
                                   &&x.Id!=id
                                   &&x.Content==request.Content,cancellationToken);

        if (questionIsExisted)
            return Result.Failure(QuestionErrors.DuplicatedQuestion);

        var question = await _context.Questions
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id,cancellationToken);
            
        if(question is null)
            return Result.Failure(QuestionErrors.NotFoundQuestion);

        question.Content= request.Content;

        var currentAnswers= question.Answers.Select(x=>x.Content).ToList();

        var newAnswers = request.Answers.Except(currentAnswers);

        foreach (var answer in newAnswers)
        {
            question.Answers.Add(new Answer { Content = answer });
        }

        question.Answers.ToList().ForEach(x => { x.IsActive = request.Answers.Contains(x.Content); });

        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
        return Result.Success();
    }
}
