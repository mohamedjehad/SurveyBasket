namespace SurveyBasket.Api.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidToken =
        new("User.InvalidToken", "Token not valid", StatusCodes.Status401Unauthorized);
    public static readonly Error NotFound =
        new("User.NotFound", "No User Was Found with the given Id",StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email is existed",StatusCodes.Status400BadRequest);
    public static readonly Error EmailNotConfirmed =
       new("User.EmailNotConfirmed", "User email not confirmed", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidCode =
       new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
    public static readonly Error EmailDuplicatedConfirmed =
       new("User.EmailDuplicatedConfirmed", "This email already confirmed", StatusCodes.Status400BadRequest);
}
