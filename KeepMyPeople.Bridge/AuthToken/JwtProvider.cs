using KeepMyPeople.Bridge.AuthToken;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KeepInTouch.API.Providers;

internal sealed class JwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(Guid id)
    {
        var claims = new Claim[]
        {
            new (ClaimTypes.NameIdentifier, id.ToString())
        };
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMonths(1),
            signingCredentials);
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}