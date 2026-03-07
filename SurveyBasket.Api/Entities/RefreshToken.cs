namespace SurveyBasket.Api.Entities;

[Owned]
public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokesOn { get; set; }
    public bool IsExpired => ExpiresOn >= CreatedOn;
    public bool IsActive => RevokesOn is null && !IsExpired;
}
