public class ApiService : IApiService
{
    private readonly HttpClient _http;
    public ApiService(HttpClient http) => _http = http;

    public async Task<Employee> ValidateEmployeeAsync(string badgeId)
    {
        return await _http.GetFromJsonAsync<Employee>($"/api/employees/validate?badgeId={badgeId}");
    }

    public async Task<ItemValidationResult> ValidateItemAsync(string barcode)
    {
        return await _http.GetFromJsonAsync<ItemValidationResult>($"/api/items/validate?barcode={barcode}");
    }
}