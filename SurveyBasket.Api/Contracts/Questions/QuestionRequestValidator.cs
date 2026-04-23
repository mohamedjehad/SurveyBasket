namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Question content is required")
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .WithMessage("Question must have more than one answer")
            .When(x => x.Answers != null);

        RuleForEach(x => x.Answers)
            .NotEmpty()
            .Must(answer => !string.IsNullOrWhiteSpace(answer))
            .WithMessage("Answer cannot be empty")
            .Length(1, 1000)
            .When(x => x.Answers != null);

        RuleFor(x => x.Answers)
            .Must(answers => answers
                .Select(answer => answer.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() == answers.Count)
            .WithMessage("Question cannot have duplicate answers")
            .When(x => x.Answers != null);
    }
}
