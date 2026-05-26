namespace YourApp.Features.Auth.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using YourApp.Features.Auth.Models;

public class MauiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtTokenService;

    public MauiAuthenticationStateProvider(
        IAuthenticationService authService,
        IJwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;

        _authService.AuthStateChanged += OnAuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var autoLoginSuccess = await _authService.AutoLoginAsync();

            if (_authService.IsAuthenticated && _authService.CurrentUser != null)
            {
                var user = CreateClaimsPrincipal(_authService.CurrentUser);
                return new AuthenticationState(user);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task LoginAsync(UserDto user, string accessToken)
    {
        var claimsPrincipal = CreateClaimsPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }

    private ClaimsPrincipal CreateClaimsPrincipal(UserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        foreach (var role in user.Roles ?? new List<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    private void OnAuthStateChanged(object sender, AuthStateChangedEventArgs e)
    {
        var claimsPrincipal = e.IsAuthenticated && e.User != null
            ? CreateClaimsPrincipal(e.User)
            : new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(claimsPrincipal)));
    }
}