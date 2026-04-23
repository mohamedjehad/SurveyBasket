using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Presistence.EntitiesConfigurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(a => a.Content).HasMaxLength(1000);
        builder.HasIndex(a=>new {a.Content,a.QuestionId}).IsUnique();
    }
}
