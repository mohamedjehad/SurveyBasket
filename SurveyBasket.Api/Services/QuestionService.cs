using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

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
        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(x => pollId == x.Id);
        if(!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.NotFoundPoll);

        var questions = await _context.Questions
            .Where(x=>x.PollId==pollId)
            .Include(q=>q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }



    public async Task<Result<QuestionResponse>> GetAsync(int pollId,int id,CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Where(x => x.Id == id&&x.PollId==pollId)
            .Include (q=>q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return question is null ?
            Result.Failure<QuestionResponse>(QuestionErrors.NotFoundQuestion)
            : Result.Success(question);
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
                            .SingleOrDefaultAsync(x => x.Id == questionId && x.PollId == pollId,cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.NotFoundQuestion);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
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
        return Result.Success();
    }
}
