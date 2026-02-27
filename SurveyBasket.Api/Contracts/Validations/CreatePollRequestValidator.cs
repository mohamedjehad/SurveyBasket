using FluentValidation;
using SurveyBasket.Api.Contracts.Requests;

namespace SurveyBasket.Api.Contracts.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(p=>p.Title).NotEmpty()
            .Length(3,100);

        RuleFor(p=>p.Description)
            .NotEmpty()
            .Length(20,500);
    }
}
