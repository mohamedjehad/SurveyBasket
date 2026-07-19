using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters request, CancellationToken cancellationToken);
    Task<QuestionResponse?> GetAsync(int pollId,int id,CancellationToken cancellationToken);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId,string userId,CancellationToken cancellationToken);
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int pollId,int id,QuestionRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(int pollId,int questionId,CancellationToken cancellationToken);
}
