// Services/EmployeeService.cs
public class EmployeeService
{
    private readonly HttpClient _httpClient;
    private readonly AuthState _authState;

    public EmployeeService(HttpClient httpClient, AuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<EmployeeValidationResult> VerifyEmployeeAsync(string badgeId)
    {
        var response = await _httpClient.GetAsync($"/api/employees/validate?badgeId={badgeId}");

        if (!response.IsSuccessStatusCode)
            return new EmployeeValidationResult(null, "Network error");

        var employee = await response.Content.ReadFromJsonAsync<Employee>();
        return employee == null
            ? new EmployeeValidationResult(null, "Employee not found")
            : new EmployeeValidationResult(employee, null);
    }
}

public record EmployeeValidationResult(Employee? Employee, string? Error);