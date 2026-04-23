namespace SurveyBasket.Api.Entities;

public class Answer:AuditableEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Question Question { get; set; } = default!;
}