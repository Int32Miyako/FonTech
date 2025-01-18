using Asp.Versioning;
using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <inheritdoc />
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="registerUserDto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult>> Register([FromForm]RegisterUserDto registerUserDto)
    {
        var ct = HttpContext.RequestAborted;
        
        var response = await _authService.RegisterAsync(registerUserDto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(_authService);
    }
    
    
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromForm]LoginUserDto loginUserDto)
    {
        var ct = HttpContext.RequestAborted;
        
        var response = await _authService.LoginAsync(loginUserDto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(_authService);
    }
}