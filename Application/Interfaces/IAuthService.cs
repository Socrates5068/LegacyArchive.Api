using LegacyArchive.Application.DTOs.Auth;

namespace LegacyArchive.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
