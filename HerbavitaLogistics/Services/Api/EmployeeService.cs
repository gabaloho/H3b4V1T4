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

public class EmployeeValidationService
{
    public async Task<ValidationResult> ValidateEmployeeAsync(string badgeId)
    {
        try
        {
            var employee = await _api.GetEmployeeByBadgeIdAsync(badgeId);
            if (employee == null || !employee.IsActive)
                return ValidationResult.Fail("Invalid or inactive badge");

            return ValidationResult.Success(employee);
        }
        catch (HttpRequestException)
        {
            return ValidationResult.Fail("Connection error");
        }
    }
}

public record ValidationResult(bool IsSuccess, Employee? Employee, string? ErrorMessage)
{
    public static ValidationResult Success(Employee e) => new(true, e, null);
    public static ValidationResult Fail(string error) => new(false, null, error);
}