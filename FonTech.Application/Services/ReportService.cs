using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain;
using FonTech.Domain.Dto;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FonTech.Application.Services;

/// <summary>
/// сервис это всего лишь прослойка от API к БД
/// В рамках его сперва запрос от апи к сервису
/// сервис к репозиторию
/// репозиторий к бд
/// и все также в обратном порядке
/// </summary>
public class ReportService : IReportService
{
    private readonly ILogger _logger;

    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IReportValidator _reportValidator;
    private readonly IMapper _mapper;

    public ReportService(IBaseRepository<Report> reportRepository, ILogger logger, IBaseRepository<User> userRepository,
        IReportValidator reportValidator, IMapper mapper)
    {
        _logger = logger;
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsByUserIdAsync(long userId, CancellationToken ct)
    {
        List<ReportDto> reports;

        try
        {
            reports = await _reportRepository.GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message); // Serilog сделает кайфы

            return new CollectionResult<ReportDto>
            {
                // best practice how to return errors for client
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }

        if (reports.Count == 0)
        {
            _logger.Warning(ErrorMessage.ReportsNotFound, reports.Count);

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
        try
        {
            // Получаем сущность из базы данных
            var reportEntity = await _reportRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (reportEntity == null)
            {
                _logger.Warning($"Отчёт с ID {id} не найден");

                return new BaseResult<ReportDto>
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }

            // Используем AutoMapper для преобразования
            var report = _mapper.Map<ReportDto>(reportEntity);

            return new BaseResult<ReportDto>(report);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message); // Логируем ошибку

            return new BaseResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }


    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto, CancellationToken ct)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, ct);

            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, ct);

            var result = _reportValidator.CreateValidator(report, user);


            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report = new Report
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = user.Id
            };

            await _reportRepository.CreateAsync(report, ct);

            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message); // Serilog сделает кайфы

            return new BaseResult<ReportDto>
            {
                // best practice how to return errors for client
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }
    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto, CancellationToken ct)
    {
        try
        {
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
            var result = _reportValidator.ValidateOnNull(report);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }
            
            report.Name = dto.Name;
            report.Description = dto.Description;

            await _reportRepository.UpdateAsync(report, ct);
            return new BaseResult<ReportDto>
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message); // Serilog сделает кайфы

            return new BaseResult<ReportDto>
            {
                // best practice how to return errors for client
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id, CancellationToken ct)
    {
        try
        {
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id, ct);
            var result = _reportValidator.ValidateOnNull(report);

            
            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            await _reportRepository.RemoveAsync(report, ct);
            return new BaseResult<ReportDto>
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message); // Serilog сделает кайфы

            return new BaseResult<ReportDto>
            {
                // best practice how to return errors for client
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }
}