// Services/JwtHandler.cs
public class JwtHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add token if available
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Refresh token if 401 and we have refresh token
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshToken = await SecureStorage.Default.GetAsync("refresh_token");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Call refresh endpoint and retry
            }
        }

        return response;
    }
}