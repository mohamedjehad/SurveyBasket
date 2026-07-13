namespace SurveyBasket.Api.Contracts.Roles;

public class RoleRequestValidator:AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Permissions)
            .NotNull()
            .NotEmpty()
            .WithMessage("Permissions cannot be null or empty");

        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You cannot add duplicate permissions to the same role")
            .When(x => x.Permissions != null);
    }
}
