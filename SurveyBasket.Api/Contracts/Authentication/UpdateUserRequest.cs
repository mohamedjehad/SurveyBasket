namespace SurveyBasket.Api.Contracts.Authentication;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles); 
