namespace SurveyBasket.Api.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(p=>p.Title).NotEmpty()
            .Length(3,100);

        RuleFor(p=>p.Summary)
            .NotEmpty()
            .Length(3,1500);

        RuleFor(p => p.StartsAt).NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(p => p.EndsAt).NotEmpty();

        RuleFor(x => x)
            .Must(HasValidDate).WithName(nameof(PollRequest.EndsAt))
            .WithMessage("{PropertyName} must be Greater than or Equal Starts Date");
    }
    private bool  HasValidDate(PollRequest pollRequest)
    {
        return pollRequest.EndsAt >= pollRequest.StartsAt;
    }
}
