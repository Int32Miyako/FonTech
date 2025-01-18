using System.Security.Claims;
using FonTech.Domain.Dto;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

public interface ITokenService
{ 
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    Task<BaseResult<TokenDto>> RefreshTokenAsync(TokenDto dto, CancellationToken ct);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}