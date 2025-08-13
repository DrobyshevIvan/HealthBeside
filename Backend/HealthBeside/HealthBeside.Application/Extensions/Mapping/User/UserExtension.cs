using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Extensions.Mapping.User;

public static class UserExtension
{
    public static GetUserDto ToGetUserDto(this ApplicationUser user)
    {
        return new GetUserDto()
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
    
}