namespace SurveyBasket.Api.Contracts.Authentication;

public record CreateUserRequest
(string FirstName,
string LastName,
string Email,
string Paswword,
IList<string> Roles);