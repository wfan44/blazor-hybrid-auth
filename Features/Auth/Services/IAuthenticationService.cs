namespace YourApp.Features.Auth.Services;

using YourApp.Features.Auth.Models;

public interface IAuthenticationService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync();
    Task LogoutAsync();
    Task<bool> AutoLoginAsync();
    bool IsAuthenticated { get; }
    UserDto CurrentUser { get; }
    event EventHandler<AuthStateChangedEventArgs> AuthStateChanged;
}

public class AuthStateChangedEventArgs : EventArgs
{
    public bool IsAuthenticated { get; set; }
    public UserDto User { get; set; }
}