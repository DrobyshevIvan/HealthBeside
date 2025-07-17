using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HealthBeside.Infrastructure.Processors;

public class AuthTokenProcessor : IAuthTokenProcessor
{
    private readonly JwtOptions _jwtOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthTokenProcessor(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
    {
        _jwtOptions = jwtOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(ApplicationUser user, List<Claim> roleClaims, IList<Claim> userClaims)
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        
        var signingCredentials = new SigningCredentials(signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.ToString())
        }.Union(userClaims).Union(roleClaims);
        
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationTimeInMinutes);
        
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials);
        
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        return (jwtToken, expires);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        
        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }

    public void WriteAuthTokenToHttpOnlyCookie(string cookieName, string token, DateTime expiration)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, token, 
            new CookieOptions{
                HttpOnly = true,
                Expires = expiration,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
    }
}