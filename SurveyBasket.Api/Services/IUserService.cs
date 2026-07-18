using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string id);
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateProfileAsync(UpdateProfileRequest request,string userId);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request,string userId);
    Task<Result> ToggleStatusAsync(string id);
    Task<Result> UnlockUser(string id);

}
