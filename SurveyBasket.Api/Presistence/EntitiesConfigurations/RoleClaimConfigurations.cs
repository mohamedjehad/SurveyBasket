using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Presistence.EntitiesConfigurations;

public class RoleClaimConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var permissions = Permissions.GetAllPermissions();
        var adminClaims = new List<IdentityRoleClaim<string>>();

        for (int i = 0; i < permissions.Count; i++)
        {
          adminClaims.Add( new IdentityRoleClaim<string>
            {
                Id = 1 + i,
                ClaimType=Permissions.Type,
                ClaimValue = permissions[i],
                RoleId=DefaultRoles.AdminRoleId
            });
        }

        builder.HasData(adminClaims);
    }
}
