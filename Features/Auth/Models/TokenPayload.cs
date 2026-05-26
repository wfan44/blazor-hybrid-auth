namespace YourApp.Features.Auth.Models;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TokenPayload
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool ExpiresWithin(TimeSpan timeSpan) => 
        ExpiresAt - DateTime.UtcNow <= timeSpan;

    public static TokenPayload FromJwt(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return new TokenPayload
            {
                UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Roles = jwtToken.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList(),
                ExpiresAt = jwtToken.ValidTo
            };
        }
        catch
        {
            return null;
        }
    }
}