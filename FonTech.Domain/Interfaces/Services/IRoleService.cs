using FonTech.Domain.Dto.Role;
using FonTech.Domain.Entity;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;
/// <summary>
/// Сервис предназначенный для управления ролей
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Получить роль по ID пользователя
    /// </summary>
    /// <param name="getRoleByUserIdDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> GetRoleAsync(GetRoleByUserIdDto getRoleByUserIdDto, CancellationToken ct);

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="createRoleDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> CreateRoleAsync(CreateRoleDto createRoleDto, CancellationToken ct);


    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="deleteRoleDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> DeleteRoleAsync(DeleteRoleDto deleteRoleDto, CancellationToken ct);


    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="updateRoleDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> UpdateRoleAsync(UpdateRoleDto updateRoleDto, CancellationToken ct);
    
    /// <summary>
    /// Применение роли к User
    /// </summary>
    /// <param name="updateRoleDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken ct);
    
    
}