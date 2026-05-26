namespace YourApp.Features.Auth.Services;

using YourApp.Features.Auth.Models;

public interface IJwtTokenService
{
    TokenPayload ParseToken(string token);
    bool IsTokenExpired(string token);
    bool IsTokenExpiringWithin(string token, TimeSpan timeSpan);
    TimeSpan GetTokenTimeRemaining(string token);
}

public class JwtTokenService : IJwtTokenService
{
    public TokenPayload ParseToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return TokenPayload.FromJwt(token);
    }

    public bool IsTokenExpired(string token)
    {
        var payload = ParseToken(token);
        return payload?.IsExpired ?? true;
    }

    public bool IsTokenExpiringWithin(string token, TimeSpan timeSpan)
    {
        var payload = ParseToken(token);
        return payload?.ExpiresWithin(timeSpan) ?? true;
    }

    public TimeSpan GetTokenTimeRemaining(string token)
    {
        var payload = ParseToken(token);
        if (payload == null)
            return TimeSpan.Zero;

        var remaining = payload.ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
}