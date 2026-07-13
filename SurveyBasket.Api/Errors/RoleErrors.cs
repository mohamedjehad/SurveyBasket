namespace SurveyBasket.Api.Errors;

public class RoleErrors
{
    public static readonly Error NotFound =
        new("Role.NotFound", "Role is not found",StatusCodes.Status404NotFound);
    public static readonly Error DuplicateRole =
        new("Role.Duplicate", "Another Role with same name is Already Exist",StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermissions =
        new("Role.InvalidPermissions", "Invalid Permissions", StatusCodes.Status400BadRequest);
}
