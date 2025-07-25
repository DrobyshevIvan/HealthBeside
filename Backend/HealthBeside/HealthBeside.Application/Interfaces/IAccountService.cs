using System.Security.Claims;
using HealthBeside.Application.Contracts;

namespace HealthBeside.Application.Interfaces;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
    Task RefreshTokenAsync(string? refreshToken);
    Task LogoutAsync(string refreshToken);
}