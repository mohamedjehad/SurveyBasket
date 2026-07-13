using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled=false,CancellationToken cancellationToken=default) =>
       await _roleManager.Roles
        .Where(x => !x.IsDefault&&(!x.IsDeleted||(includeDisabled.HasValue&&includeDisabled.Value)))
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

        return Result.Success(response);     
    }

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        var roleIsExist = await _roleManager.RoleExistsAsync(request.Name);

        if (roleIsExist)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicateRole);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if(result.Succeeded)
        {
            var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string>
            {
                ClaimType = Permissions.Type,
                ClaimValue = x,
                RoleId = role.Id
            });

            await _context.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
            return Result.Success(response);
        }
        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id,RoleRequest request)
    {
        var roleIsExist = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if(roleIsExist )
            return Result.Failure(RoleErrors.DuplicateRole);

        var role = await _roleManager.FindByIdAsync(id);

        if( role == null )
            return Result.Failure(RoleErrors.NotFound);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);

        role.Name= request.Name;

        var result= await _roleManager.UpdateAsync(role);

        if(result.Succeeded)
        {
            var currentPermissions= await _context.RoleClaims
                .Where(x=>x.RoleId==id&&Permissions.Type==x.ClaimType)
                .Select(x=>x.ClaimValue)
                .ToListAsync();

            var newpermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
            {
                ClaimType = Permissions.Type,
                ClaimValue = x,
                RoleId = role.Id
            });

            var removedPermissions= currentPermissions.Except(request.Permissions);

            await _context.RoleClaims
                .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();

            await _context.AddRangeAsync(newpermissions);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDeleted = !role.IsDeleted;

        await _roleManager.UpdateAsync(role); 

        return Result.Success();
    }
}
