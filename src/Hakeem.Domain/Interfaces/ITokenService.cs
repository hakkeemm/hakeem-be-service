using Hakeem.Domain.Entities;

namespace Hakeem.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}
