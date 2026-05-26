namespace YourApp.Features.Auth.Services;

public interface ISecureStorageService
{
    Task SaveAsync(string key, string value);
    Task<string> GetAsync(string key);
    Task RemoveAsync(string key);
    Task RemoveAllAsync();
}

public class SecureStorageService : ISecureStorageService
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string UserKey = "user_data";

    public async Task SaveAsync(string key, string value)
    {
        try
        {
            await SecureStorage.Default.SetAsync(key, value);
        }
        catch (NotSupportedException)
        {
            // Fallback to preferences if SecureStorage not available
            Preferences.Default.Set(key, value);
        }
    }

    public async Task<string> GetAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key);
        }
        catch (NotSupportedException)
        {
            return Preferences.Default.Get(key, null);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            SecureStorage.Default.Remove(key);
        }
        catch (NotSupportedException)
        {
            Preferences.Default.Remove(key);
        }
    }

    public async Task RemoveAllAsync()
    {
        SecureStorage.Default.RemoveAll();
        Preferences.Default.Clear();
    }
}