using SurveyBasket.Api.Contracts.Polls.Answers;

namespace SurveyBasket.Api.Contracts.Questions;

public record QuestionResponse
(int Id,
 string Content
,IEnumerable<AnswerResponse> Answers);
