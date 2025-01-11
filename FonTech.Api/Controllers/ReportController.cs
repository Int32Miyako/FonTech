using Asp.Versioning;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

/// <inheritdoc />
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    
    /// <inheritdoc />
    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    /// <summary>
    /// Получение отчёта по его ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("byOwnId/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _reportService.GetReportByIdAsync(id, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Получение всех отчётов по ID пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("byUserId/{userId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)] // документация возможных ошибок
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId)
    {
        //  HttpContext.RequestAborted это ct
        var ct = HttpContext.RequestAborted;
        
        var response = await _reportService.GetReportsByUserIdAsync(userId, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    /// <summary>
    /// Удаление отчёта
    /// </summary>
    /// <param name="id"></param>
    /// 
    /// <remarks>
    /// Request for delete report:
    /// 
    ///     DELETE
    ///     {
    ///        "id": 1
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если отчёт создался</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)] // документация возможных ошибок
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Delete(long id)
    {
        var ct = HttpContext.RequestAborted;
        
        var response = await _reportService.DeleteReportAsync(id, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    /// <summary>
    /// Создание отчёта
    /// </summary>
    /// <param name="createReportDto"></param>
    /// 
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///        "name": "Report #1",
    ///        "description": "Test report",
    ///        "userId": 1
    ///     }
    /// 
    /// </remarks>
    /// 
    /// <response code="200">Если отчёт создался</response>
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK)] // документация возможных ошибок
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromForm]CreateReportDto createReportDto)
    {
        var ct = HttpContext.RequestAborted;
        
        var response = await _reportService.CreateReportAsync(createReportDto , ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    // нельзя заменить на пост т к
    /// <summary>
    /// Обновление отчёта
    /// </summary>
    /// <param name="updateReportDto"></param>
    /// <returns></returns>
    [HttpPut("")] // сколько бы раз не сделал => один и тот же результат
    [ProducesResponseType(StatusCodes.Status200OK)] // документация возможных ошибок
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromForm]UpdateReportDto updateReportDto)
    {
        var ct = HttpContext.RequestAborted;
        
        var response = await _reportService.UpdateReportAsync(updateReportDto, ct);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}