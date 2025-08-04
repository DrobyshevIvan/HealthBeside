using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Interfaces;

public interface IAccountService
{
    Task<GetUserInfoDto> GetUserInfoAsync(
        Guid currentUserId,
        Guid requestedUserId,
        CancellationToken cancellationToken = default);
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
    Task RefreshTokenAsync(string? refreshToken);
    Task LogoutAsync(string refreshToken);
}