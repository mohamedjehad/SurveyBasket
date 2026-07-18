using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class CreateUserRequestValidator:AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Paswword)
            .Matches(RegexPatterns.Password)
            .WithMessage("Password Should be at least 8 digits, and contain Lowercase, Nonalphanumeric and Uppercase");

        RuleFor(x => x.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Roles)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You cannot add duplicate role to the same user")
            .When(x=>x.Roles!=null);

    }
}
