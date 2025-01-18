using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FonTech.Application.Resources;
using FonTech.Domain.Dto;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using FonTech.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FonTech.Application.Services;

public class TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository) : ITokenService
{
    private readonly string _jwtKey = options.Value.JwtKey;
    private readonly string _issuer = options.Value.Issuer;
    private readonly string _audience = options.Value.Audience;

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken =
            new JwtSecurityToken(_issuer, _audience, claims, 
            null, DateTime.UtcNow.AddMinutes(10), 
            credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumbers = new byte[32];

        using var randomNumberGenerator = RandomNumberGenerator.Create();
        
        randomNumberGenerator.GetBytes(randomNumbers);

        return Convert.ToBase64String(randomNumbers);
    }

    /// <summary>
    /// достать из клаймов юзер нейм
    /// получить нейм из базы данных
    /// проверять остальные данные пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<BaseResult<TokenDto>> RefreshTokenAsync(TokenDto dto, CancellationToken ct)
    {
        string accessToken = dto.AccessToken;
        string refreshToken = dto.RefreshToken;

        var claimPrincipal = GetPrincipalFromExpiredToken(accessToken);
        var userName = claimPrincipal.Identity?.Name;


        var user = await userRepository.GetAll()
            .Include(x 
                => x.UserToken) // т к связь один к одному мы  делаем проверку свяанных данных и вытаскиваем юзер токен
            .FirstOrDefaultAsync(x => x.Login == userName, cancellationToken: ct);


        if (user == null || user.UserToken.RefreshToken != refreshToken
            || user.UserToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.InvalidClientRequest
            };
        }

        var newAccessToken = GenerateAccessToken(claimPrincipal.Claims);
        var newRefreshToken = GenerateRefreshToken();

        user.UserToken.RefreshToken = newRefreshToken;
        await userRepository.UpdateAsync(user, ct);

        return new BaseResult<TokenDto>
        {
            Data = new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }
        };
    }
    
    /// <summary>
    /// метод который в принципе проверяет за валидацию токена
    /// и возвращает нам клаймы
    ///
    ///
    /// нужен в секции рефреш токена
    /// </summary>
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            // валидация пользователя
            ValidateAudience = true,
            // валидация сервера
            ValidateIssuer = true,
            // валидация ключа издателя
            ValidateIssuerSigningKey = true,
            // проверовка что правильный енкодинг
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),

            ValidateLifetime = true,
            
            ValidAudience = _audience,
            ValidIssuer = _issuer,
        };

        var tokenHandler = new JwtSecurityTokenHandler();


        var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException(ErrorMessage.InvalidSecurityToken);
        
        
        return claimsPrincipal;
    }
}