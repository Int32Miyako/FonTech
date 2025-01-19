using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Producer;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;
using FonTech.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace FonTech.Application.Services;

/// <summary>
/// сервис это всего лишь прослойка от API к БД
/// В рамках его сперва запрос от апи к сервису
/// сервис к репозиторию
/// репозиторий к бд
/// и все также в обратном порядке
/// </summary>
public class ReportService(
    IBaseRepository<Report> reportRepository,
    ILogger logger,
    IBaseRepository<User> userRepository,
    IMessageProducer messageProducer,
    IOptions<RabbitMqSettings> rabbitMqSettings,
    IReportValidator reportValidator,
    IMapper mapper)
    : IReportService
{
    
    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsByUserIdAsync(long userId, CancellationToken ct)
    {
        var reports = await reportRepository.GetAll()
            .Where(x => x.UserId == userId)
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
            .ToListAsync(ct);


        if (reports.Count == 0)
        {
            logger.Warning(ErrorMessage.ReportsNotFound, reports.Count);

            return new CollectionResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        // не прописываем ErrorMessage!!!
        return new CollectionResult<ReportDto>
        {
            Data = reports,
            Count = reports.Count
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id, CancellationToken ct)
    {
        // Получаем сущность из базы данных
        var reportEntity = await reportRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (reportEntity == null)
        {
            logger.Warning($"Отчёт с ID {id} не найден");

            return new BaseResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            };
        }

        // Используем AutoMapper для преобразования
        var report = mapper.Map<ReportDto>(reportEntity);

        return new BaseResult<ReportDto>(report);
    }


    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto, CancellationToken ct)
    {
        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, ct);

        var report = await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, ct);

        var result = reportValidator.CreateValidator(report, user);


        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        report = new Report
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = user!.Id
        };
        await reportRepository.CreateAsync(report, ct);
        //await reportRepository.SaveChangesAsync();
        
        messageProducer.SendMessage(
            report, 
            rabbitMqSettings.Value.RoutingKey, 
            rabbitMqSettings.Value.ExchangeName
            );
            
            
            
        return new BaseResult<ReportDto>
        {
            Data = mapper.Map<ReportDto>(report)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto, CancellationToken ct)
    {
        var report = await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
        var result = reportValidator.ValidateOnNull(report);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        report!.Name = dto.Name;
        report.Description = dto.Description;

        await reportRepository.UpdateAsync(report, ct);
        return new BaseResult<ReportDto>
        {
            Data = mapper.Map<ReportDto>(report)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id, CancellationToken ct)
    {
        var report = await reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id, ct);
        var result = reportValidator.ValidateOnNull(report);
        
        
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        await reportRepository.RemoveAsync(report!, ct);
        return new BaseResult<ReportDto>
        {
            Data = mapper.Map<ReportDto>(report)
        };
    }
}