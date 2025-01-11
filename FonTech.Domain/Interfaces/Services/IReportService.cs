using FonTech.Domain.Dto;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

/// <summary>
/// Сервис отвечающий за работу доменной части отчета
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Получение всех отчетов пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<CollectionResult<ReportDto>> GetReportsByUserIdAsync(long userId, CancellationToken ct);


    /// <summary>
    /// Получение отчёта по Id отчёта
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> GetReportByIdAsync(long id, CancellationToken ct);
    
    /// <summary>
    /// Создание отчёта по классу CreateReportDto
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto, CancellationToken ct);
    
    /// <summary>
    /// Обновление отчета по классу CreateReportDto
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto, CancellationToken ct);

    /// <summary>
    /// Удаление отчёта по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id, CancellationToken ct);
    
    
}