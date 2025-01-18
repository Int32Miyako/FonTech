using Asp.Versioning;
using FonTech.Domain.Dto;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]

public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenService"></param>
    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenDto"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _tokenService.RefreshTokenAsync(tokenDto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}