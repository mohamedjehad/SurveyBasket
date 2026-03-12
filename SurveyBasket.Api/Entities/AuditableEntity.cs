namespace SurveyBasket.Api.Entities;

public class AuditableEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public ApplicationUser CreatedBy { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
