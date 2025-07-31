namespace HealthBeside.Domain.Models.Users;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    
    private RefreshToken() { }  

    public static (string? Error, RefreshToken? refreshToken) Create(string token,
        DateTime expiresOnUtc,
        Guid userId)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(token))
            errors.Add("Refresh token cannot be empty.");

        if (DateTime.UtcNow > expiresOnUtc || expiresOnUtc == DateTime.MinValue)
            errors.Add("Refresh token expires or missing value.");
        
        if (userId == Guid.Empty)
            errors.Add("User id cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            Token = token,
            ExpiresOnUtc = expiresOnUtc,
            UserId = userId,
        };
        
        return (null,refreshToken);
    }
}