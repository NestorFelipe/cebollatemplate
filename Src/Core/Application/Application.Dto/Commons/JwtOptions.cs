namespace Application.Dto.Commons;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audence { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
