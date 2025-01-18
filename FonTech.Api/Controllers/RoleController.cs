using System.Net.Mime;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    
    /// <inheritdoc />
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    /// <summary>
    /// Создание роли по имени роли
    /// </summary>
    /// <remarks>
    /// Sample request
    /// 
    ///     Post
    ///     {
    ///        "name" : "Admin"
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если роль удалена</response>
    /// <response code="400">Если роль не была найдена</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<Role>>> Create([FromBody] CreateRoleDto dto)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _roleService.CreateRoleAsync(dto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    /// <summary>
    /// Удаление роли по Id роли, или имени роли, если есть
    /// если имя = null или пустое значение то удаление будет по Id
    /// </summary>
    /// <remarks>
    /// Sample request
    /// 
    ///     Delete
    ///     {
    ///        "id": 1,
    ///        "name" : "Admin"
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если роль удалена</response>
    /// <response code="400">Если роль не была найдена</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<Role>>> Delete([FromBody] DeleteRoleDto dto)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _roleService.DeleteRoleAsync(dto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Обновление роли по Id роли, или имени роли, если такая есть
    /// если имя null или пустое значение то обновление будет по Id
    /// </summary>
    /// <remarks>
    /// Sample request
    /// 
    ///     Put
    ///     {
    ///        "id": 1,
    ///        "name" : "Admin"
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если роль обновилась</response>
    /// <response code="400">Если роль не была найдена</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<Role>>> Update([FromBody] UpdateRoleDto dto)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _roleService.UpdateRoleAsync(dto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    /// <summary>
    /// Добавление роли пользователю
    /// </summary>
    /// <remarks>
    /// Sample request
    /// 
    ///     Post
    ///     {
    ///        "login": "Vendetta Hoe",
    ///        "roleName" : "Admin"
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если роль обновилась</response>
    /// <response code="400">Если роль не была найдена</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost("addRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<Role>>> AddRoleToUser([FromBody] UserRoleDto dto)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _roleService.AddRoleForUserAsync(dto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

}