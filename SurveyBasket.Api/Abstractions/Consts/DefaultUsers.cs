namespace SurveyBasket.Api.Abstractions.Consts;

public static class DefaultUsers
{
    public const string AdminId = "019f3f7e-73dc-7820-8ea6-280018809133";
    public const string AdminEmail = "admin@survey-basket.com";
    public const string AdminPassword = "P@ssword123";

    // Pre-computed via PasswordHasher<object>.HashPassword once — never regenerate inside HasData().
    // To rotate: run the HashGen tool, update this constant, then add a new migration.
    public const string AdminPasswordHash = "AQAAAAEAACcQAAAAEO4JCMDgMsOsSThzdm4NvV/9um9LTXgwBXqy3VtZXEMpMOJZLjaAOgsA2ir3QJ2OBg==";
    public const string AdminSecurityStamp = "84B03BB714C74CC1AB3A8A0F4EBC983D";
    public const string AdminConcurrencyStamp = "019f3f7e-73dc-7820-8ea6-280130d29750";
}
