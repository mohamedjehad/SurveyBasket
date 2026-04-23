namespace SurveyBasket.Api.Entities;

public sealed class Question:AuditableEntity
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ICollection<Answer> Answers { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public Poll Poll { get; set; } = default!;
}
