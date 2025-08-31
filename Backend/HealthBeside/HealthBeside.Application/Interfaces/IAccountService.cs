using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = HealthBeside.Application.Contracts.LoginRequest;
using RegisterRequest = HealthBeside.Application.Contracts.RegisterRequest;

namespace HealthBeside.Application.Interfaces;

public interface IAccountService
{
    Task<GetUserInfoDto> GetUserInfoAsync(
        Guid currentUserId,
        Guid requestedUserId,
        CancellationToken cancellationToken = default);
    Task RegisterAsync(RegisterRequest request);
    Task AssignRoleAsync(Guid adminUserId, Guid targetUserId, Guid newRoleId);
    Task LoginAsync(LoginRequest request);
    Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal,
        CancellationToken cancellationToken = default);
    Task RefreshTokenAsync(string? refreshToken,
        CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken,
        CancellationToken cancellationToken = default);
}