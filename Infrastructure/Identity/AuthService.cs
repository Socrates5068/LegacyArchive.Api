using FluentValidation;
using LegacyArchive.Application.DTOs.Auth;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LegacyArchive.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<LoginRequest> _validator;
    private readonly JwtTokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IValidator<LoginRequest> validator,
        JwtTokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _validator = validator;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new InvalidCredentialsException();

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Login attempt for locked out user {UserId}.", user.Id);
            throw new InvalidCredentialsException();
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new InvalidCredentialsException();
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenService.CreateToken(user, roles);
    }
}
