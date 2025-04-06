// Services/AuthState.cs
public sealed class AuthState
{
    private static readonly Lazy<AuthState> _instance = new Lazy<AuthState>(() => new AuthState());
    public static AuthState Current => _instance.Value;

    public UserInfo? CurrentUser { get; private set; }
    public string? AuthToken { get; private set; }

    public void Login(UserInfo user, string token)
    {
        CurrentUser = user;
        AuthToken = token;
        SecureStorage.Default.SetAsync("auth_token", token);
    }

    public void Logout()
    {
        CurrentUser = null;
        AuthToken = null;
        SecureStorage.Default.Remove("auth_token");
    }

    public static async Task InitializeAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            // Validate token with API if needed
            Current.AuthToken = token;
        }
    }
}

public record UserInfo(string Id, string Name, string Role);

public class AuthState
{
    public Employee? CurrentEmployee { get; private set; }

    public event Action? OnChange;

    public void Login(Employee employee)
    {
        CurrentEmployee = employee;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentEmployee = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}