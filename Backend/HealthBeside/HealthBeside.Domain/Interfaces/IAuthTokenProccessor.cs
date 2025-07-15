using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Interfaces;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(ApplicationUser user);
    string GenerateRefreshToken();
    void WriteAuthTokenToHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}