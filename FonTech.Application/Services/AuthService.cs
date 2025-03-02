using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Databases;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;


namespace FonTech.Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IBaseRepository<UserRole> userRoleRepository,
    IMapper mapper,
    IBaseRepository<UserToken> tokenRepository,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
    {
        // проверка что пароли не совпадают
        if (dto.Password != dto.PasswordConfirm)
        {
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.PasswordNotEqualsPasswordConfirm,
                ErrorCode = (int)ErrorCodes.PasswordNotEqualsPasswordConfirm
            };
        }


        // проверка на то, есть ли такой юзер уже в базе
        var user = await userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Login == dto.Login, ct);
        
        if (user != null)
        {
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.UserAlreadyExists,
                ErrorCode = (int)ErrorCodes.UserAlreadyExists
            };
        }

        // если все проверки прошли идём заносить данные в таблицу
        var hashUserPassword = HashPassword(dto.Password);

        
        
        await using (var transaction = await unitOfWork.BeginTransactionAsync(ct))
        {
            try
            {
                user = new User
                {
                    Login = dto.Login,
                    Password = hashUserPassword
                };
                
                
                await userRepository.CreateAsync(user, ct);
                await userRepository.SaveChangesAsync(ct);
                
                var role = await roleRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), ct);

                if (role == null)
                {
                    await transaction.RollbackAsync(ct);
                    return new BaseResult<UserDto>
                    {
                        ErrorMessage = ErrorMessage.RoleNotFound,
                        ErrorCode = (int)ErrorCodes.RoleNotFound
                    };
                }
                
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                
                await userRoleRepository.CreateAsync(userRole, ct);
                await userRoleRepository.SaveChangesAsync(ct);
                
                
                await transaction.CommitAsync(ct);
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                Console.WriteLine(ex);
            }
        }
        
        return new BaseResult<UserDto>
        {
            Data = mapper.Map<UserDto>(user)
        };
    }

    public async Task<BaseResult<TokenDto>> LoginAsync(LoginUserDto loginUserDto, CancellationToken ct)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == loginUserDto.Login, ct);

        if (user == null)
        {
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }


        if (!IsVerifyPassword(user.Password, loginUserDto.Password))
        {
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.WrongPassword,
                ErrorCode = (int)ErrorCodes.WrongPassword
            };
        }

        var userToken = await tokenRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);


        var userRoles = user.Roles;
        var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.Login));
        
        //var claims = new List<Claim>
        //{
        //    new(ClaimTypes.Name, user.Login),
        //    new(ClaimTypes.Role, "User")
        //};

        var accessToken = tokenService.GenerateAccessToken(claims);


        var refreshToken = tokenService.GenerateRefreshToken();


        if (userToken == null)
        {
            userToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await tokenRepository.CreateAsync(userToken, ct);
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await tokenRepository.UpdateAsync(userToken, ct);
        }

        return new BaseResult<TokenDto>
        {
            Data = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(bytes);
    }

    private static bool IsVerifyPassword(string userPasswordHash, string userPassword)
    {
        var hash = HashPassword(userPassword);
        return userPasswordHash == hash;
    }
}