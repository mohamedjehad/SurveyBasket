using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid Email/Password",400);
    public static readonly Error InvalidToken =
        new("User.InvalidToken", "Token not valid",400);
    public static readonly Error NotFound =
        new("User.NotFound", "No User Was Found with the given Id",404);
}
