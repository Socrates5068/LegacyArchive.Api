namespace LegacyArchive.Application.DTOs.Auth;

public record LoginResponse(string AccessToken, DateTime ExpiresAt, string TokenType = "Bearer");
