using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser>userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
       var user= await _userManager
            .Users
            .Where(x=>x.Id==userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest request,string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        request.Adapt(user);

        await _userManager.UpdateAsync(user!);

        return Result.Success();
    }
}
