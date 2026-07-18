using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Users;
using System.Reflection.Metadata.Ecma335;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IRoleService roleService) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IRoleService _roleService = roleService;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await (from u in _context.Users join
               ur in _context.UserRoles
               on u.Id equals ur.UserId join
               r in _context.Roles
               on ur.RoleId equals r.Id into roles
               where !roles.Any(x => x.Name == DefaultRoles.Member)
               select new
               {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.IsDisabled,
                   Roles = roles.Select(x => x.Name)
               }).GroupBy(u => new {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.IsDisabled
               }).Select(u =>
               new UserResponse(
                   u.Key.Id,
                   u.Key.FirstName,
                   u.Key.LastName,
                   u.Key.Email,
                   u.Key.IsDisabled,
                   u.SelectMany(x => x.Roles)
                   ))
               .ToListAsync(cancellationToken);


    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return Result.Failure<UserResponse>(UserErrors.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>(); //configured in mapping config

        //u could build the response manually here

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);

        if (isEmailExist)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRole);

        var user = request.Adapt<ApplicationUser>();  //configured in mapping config

        var result = await _userManager.CreateAsync(user, request.Paswword);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var isEmailExisted = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id);

        if (isEmailExisted)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure(UserErrors.InvalidRole);

        user = request.Adapt(user);//configured in mapping config

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRoles = request.Roles.Except(currentRoles);
            var toDeleteRoles = currentRoles.Except(request.Roles);

            await _userManager.RemoveFromRolesAsync(user, toDeleteRoles);
            await _userManager.AddToRolesAsync(user, newRoles);

            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _userManager
             .Users
             .Where(x => x.Id == userId)
             .ProjectToType<UserProfileResponse>()
             .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest request, string userId)
    {
        //var user = await _userManager.FindByIdAsync(userId);

        //request.Adapt(user);

        //await _userManager.UpdateAsync(user!);

        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(x => x.LastName, request.LastName)
                    );

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        user.IsDisabled = !user.IsDisabled;

      var result= await _userManager.UpdateAsync(user);

        if(result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> UnlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        await _userManager.SetLockoutEndDateAsync(user, null);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


}
