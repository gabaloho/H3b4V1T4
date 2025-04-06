// Extensions/ApiServiceExtensions.cs
public static class ApiServiceExtensions
{
    public static IServiceCollection AddApiService(this IServiceCollection services)
    {
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => !x.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://your-api.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(15));

        return services;
    }
}