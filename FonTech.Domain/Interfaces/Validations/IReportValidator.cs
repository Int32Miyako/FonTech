using FonTech.Domain.Dto;
using FonTech.Domain.Entity;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Validations;

// проверяет существует ли пользователь
public interface IReportValidator : IBaseValidator<Report>
{
    /// <summary>
    /// Проверяется наличие отчёта, если отчёт с переданным названием есть в бд,
    /// то создать точно такой же нельзя, если с UserId пользователь не найден,
    /// то такого пользователя нет
    /// </summary>
    /// <param name="report"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    BaseResult CreateValidator(Report report, User user);
    
    
}