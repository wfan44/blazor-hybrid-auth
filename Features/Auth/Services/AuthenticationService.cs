namespace YourApp.Features.Auth.Services;

using YourApp.Features.Auth.Models;
using System.Diagnostics;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorage;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConnectivity _connectivity;
    private readonly ITokenBackupService _tokenBackupService;
    private readonly string _apiBaseUrl;
    private readonly ILogger<AuthenticationService> _logger;

    private UserDto _currentUser;
    private string _accessToken;
    private string _refreshToken;

    public event EventHandler<AuthStateChangedEventArgs> AuthStateChanged;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken) && 
                                   !_jwtTokenService.IsTokenExpired(_accessToken);
    
    public UserDto CurrentUser => _currentUser;

    public AuthenticationService(
        IApiService apiService,
        ISecureStorageService secureStorage,
        IJwtTokenService jwtTokenService,
        IConnectivity connectivity,
        ITokenBackupService tokenBackupService,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _apiService = apiService;
        _secureStorage = secureStorage;
        _jwtTokenService = jwtTokenService;
        _connectivity = connectivity;
        _tokenBackupService = tokenBackupService;
        _logger = logger;
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://api.example.com";
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            if (!IsNetworkAvailable())
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "No internet connection available" 
                };

            var url = $"{_apiBaseUrl}/api/auth/login";
            var response = await _apiService.PostAsync<AuthResponse>(url, request);

            if (response?.Success == true)
            {
                await StoreTokensAsync(response.AccessToken, response.RefreshToken);
                await _tokenBackupService.BackupTokensAsync(response.AccessToken, response.RefreshToken, response.ExpiresIn);
                _currentUser = response.User;
                _accessToken = response.AccessToken;
                _refreshToken = response.RefreshToken;

                var json = System.Text.Json.JsonSerializer.Serialize(response.User);
                await _secureStorage.SaveAsync("user_data", json);

                RaiseAuthStateChanged(true);
                _logger.LogInformation($"User logged in: {response.User.Email}");
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Login failed: {ex.Message}" 
            };
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync()
    {
        try
        {
            if (!IsNetworkAvailable())
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "No internet connection available" 
                };

            if (string.IsNullOrWhiteSpace(_refreshToken))
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "No refresh token available" 
                };

            var request = new RefreshTokenRequest
            {
                AccessToken = _accessToken,
                RefreshToken = _refreshToken
            };

            var url = $"{_apiBaseUrl}/api/auth/refresh";
            var response = await _apiService.PostAsync<AuthResponse>(url, request);

            if (response?.Success == true)
            {
                await StoreTokensAsync(response.AccessToken, response.RefreshToken);
                await _tokenBackupService.BackupTokensAsync(response.AccessToken, response.RefreshToken, response.ExpiresIn);
                _accessToken = response.AccessToken;
                _refreshToken = response.RefreshToken;
                _currentUser = response.User;
                RaiseAuthStateChanged(true);
                _logger.LogInformation("Token refreshed successfully");
            }
            else
            {
                await LogoutAsync();
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Token refresh error: {ex.Message}");
            await LogoutAsync();
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Token refresh failed: {ex.Message}" 
            };
        }
    }

    public async Task<bool> AutoLoginAsync()
    {
        try
        {
            _accessToken = await _secureStorage.GetAsync("access_token");
            _refreshToken = await _secureStorage.GetAsync("refresh_token");

            if (string.IsNullOrWhiteSpace(_accessToken))
            {
                var (backupAccessToken, backupRefreshToken) = await _tokenBackupService.RestoreTokensAsync();
                if (!string.IsNullOrWhiteSpace(backupAccessToken))
                {
                    _accessToken = backupAccessToken;
                    _refreshToken = backupRefreshToken;
                    _logger.LogInformation("Restored tokens from offline backup");
                }
                else
                {
                    return false;
                }
            }

            if (_jwtTokenService.IsTokenExpired(_accessToken))
            {
                if (!string.IsNullOrWhiteSpace(_refreshToken) && IsNetworkAvailable())
                {
                    var response = await RefreshTokenAsync();
                    return response?.Success == true;
                }
                else
                {
                    await LogoutAsync();
                    return false;
                }
            }

            var userJson = await _secureStorage.GetAsync("user_data");
            if (!string.IsNullOrWhiteSpace(userJson))
            {
                _currentUser = System.Text.Json.JsonSerializer.Deserialize<UserDto>(userJson);
            }

            RaiseAuthStateChanged(true);
            _logger.LogInformation("Auto-login successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Auto-login error: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && IsNetworkAvailable())
            {
                try
                {
                    var url = $"{_apiBaseUrl}/api/auth/logout";
                    await _apiService.PostAsync(url, new { });
                }
                catch
                {
                    // Ignore logout endpoint errors
                }
            }

            _accessToken = null;
            _refreshToken = null;
            _currentUser = null;
            await _secureStorage.RemoveAllAsync();
            await _tokenBackupService.ClearBackupAsync();

            RaiseAuthStateChanged(false);
            _logger.LogInformation("User logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Logout error: {ex.Message}");
        }
    }

    private async Task StoreTokensAsync(string accessToken, string refreshToken)
    {
        await _secureStorage.SaveAsync("access_token", accessToken);
        await _secureStorage.SaveAsync("refresh_token", refreshToken);
    }

    private bool IsNetworkAvailable()
    {
        return _connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    private void RaiseAuthStateChanged(bool isAuthenticated)
    {
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs
        {
            IsAuthenticated = isAuthenticated,
            User = _currentUser
        });
    }
}