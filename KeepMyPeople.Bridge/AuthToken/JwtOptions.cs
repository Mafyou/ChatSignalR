namespace KeepMyPeople.Bridge.AuthToken;

public record JwtOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SecretKey { get; set; }
}