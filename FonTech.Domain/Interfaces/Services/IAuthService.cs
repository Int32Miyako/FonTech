using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

/// <summary>
/// Сервис для авторизации и регистрации
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken ct);

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> LoginAsync(LoginUserDto loginUserDto, CancellationToken ct);
}