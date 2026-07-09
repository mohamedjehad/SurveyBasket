using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Presistence.EntitiesConfigurations;

public class UserRoleConfigurations : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {

        builder.HasData(
           new IdentityUserRole<string>
           {
               UserId=DefaultUsers.AdminId,
               RoleId=DefaultRoles.AdminRoleId
           });
    }
}
