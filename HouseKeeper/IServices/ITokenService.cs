using System.Security.Claims;

namespace HouseKeeper.IServices
{
    public interface ITokenService
    {
        string GenerateToken(string userId);
        ClaimsPrincipal ValidateToken(string token);
    }
}
