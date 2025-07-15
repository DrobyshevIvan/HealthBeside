using HealthBeside.Domain.Contracts;

namespace HealthBeside.Domain.Interfaces;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task RefreshTokenAsync(string? refreshToken);
}