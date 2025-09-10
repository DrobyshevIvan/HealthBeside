using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = HealthBeside.Application.Contracts.LoginRequest;
using RegisterRequest = HealthBeside.Application.Contracts.RegisterRequestBase;

namespace HealthBeside.Application.Interfaces;

public interface IAccountService
{
    Task<GetUserInfoDto> GetUserInfoAsync(
        Guid currentUserId,
        Guid requestedUserId,
        CancellationToken cancellationToken = default);
    Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);
    Task AssignRoleAsync(Guid adminUserId, Guid targetUserId, Guid newRoleId);
    List<RoleDto> GetAvailableRoles();
    Task LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken = default);
    Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal,
        CancellationToken cancellationToken = default);
    Task RefreshTokenAsync(string? refreshToken,
        CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountAsync(Guid userId, ClaimsPrincipal currentUser,
        CancellationToken cancellationToken = default);
}