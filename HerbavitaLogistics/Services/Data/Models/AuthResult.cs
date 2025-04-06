// Models/AuthResult.cs
public record AuthResult(bool IsSuccess, Employee? Employee, string? ErrorMessage);

// Models/ItemValidationResult.cs
public record ItemValidationResult(string ItemCode, string Name, bool IsValid);