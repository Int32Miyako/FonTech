using FonTech.Application.Resources;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace FonTech.Application.Services;

public class RoleService(
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IBaseRepository<UserRole> userRoleRepository
    )
    : IRoleService
{
    public Task<BaseResult<Role>> GetRoleAsync(GetRoleByUserIdDto getRoleByUserIdDto, CancellationToken ct)
    {
        
        
        throw new NotImplementedException();
    }

    public async Task<BaseResult<Role>> CreateRoleAsync(CreateRoleDto createRoleDto, CancellationToken ct)
    {
        var role = await roleRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == createRoleDto.Name, ct);

        if (role != null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleAlreadyExists,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExists
            };
        }
        
        
        return new BaseResult<Role>
        {
            Data = await roleRepository.CreateAsync(new Role
            {
                Name = createRoleDto.Name

            }, ct)
        };
    }

    public async Task<BaseResult<Role>> DeleteRoleAsync(DeleteRoleDto deleteRoleDto, CancellationToken ct)
    {
        Role? role;
        
        if(!string.IsNullOrEmpty(deleteRoleDto.Name))
            role = await roleRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == deleteRoleDto.Name, ct);
        else
        {
            role = await roleRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == deleteRoleDto.Id, ct);
        }
        
        if (role == null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

        await roleRepository.RemoveAsync(role, ct);
        return new BaseResult<Role>
        {
            Data = role
        };
    }

    public async Task<BaseResult<Role>> UpdateRoleAsync(UpdateRoleDto updateRoleDto, CancellationToken ct)
    {
        Role? role;
        
        if(!string.IsNullOrEmpty(updateRoleDto.Name))
            role = await roleRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == updateRoleDto.Name, ct);
        else
        {
            role = await roleRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == updateRoleDto.Id, ct);
        }
        
        if (role == null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

        await roleRepository.UpdateAsync(role, ct);
        return new BaseResult<Role>
        {
            Data = role
        };
    }


    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken ct)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles) // подгружаем роли пользователя
            .FirstOrDefaultAsync(x => x.Login == dto.Login, ct);
        if (user == null)
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }
        
        
        var role = await roleRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == dto.RoleName, ct);
        
        if (role == null)
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

        var roleNames = user.Roles.Select(x => x.Name).ToArray();

        if (roleNames.Any(x => x == dto.RoleName)) // если все не равны то будет true
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserAlreadyExistsThisRole,
                ErrorCode = (int)ErrorCodes.UserAlreadyExistsThisRole
            };
        }
        
        
        var userRole = new UserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        };

        
        await userRoleRepository.CreateAsync(userRole, ct);

        
        return new BaseResult<UserRoleDto>
        {
            Data = dto
        };
    }
}