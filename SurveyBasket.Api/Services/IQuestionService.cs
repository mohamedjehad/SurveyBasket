using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    public Task<IEnumerable<QuestionResponse>> GetAllAsync(int pollId,CancellationToken cancellationToken);
    public Task<QuestionResponse?> GetAsync(int pollId,int id,CancellationToken cancellationToken);
    public Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId,string userId,CancellationToken cancellationToken);
    public Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken);
    public Task<Result> UpdateAsync(int pollId,int id,QuestionRequest request, CancellationToken cancellationToken);
    public Task<Result> ToggleStatusAsync(int pollId,int questionId,CancellationToken cancellationToken);
}
