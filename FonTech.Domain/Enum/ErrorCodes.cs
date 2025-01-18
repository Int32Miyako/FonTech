namespace FonTech.Domain.Enum;

public enum ErrorCodes
{
    InternalServerError = 10,
    // 0 - 10 - Business Entities
    // 11 - 20 - User
    ReportsNotFound = 0,
    ReportNotFound = 1,
    ReportAlreadyExists = 2,
    
    UserNotFound = 11,
    UserAlreadyExists = 12,
    UserUnauthorizedAccess = 13,
    
    
    // для регистрации
    PasswordNotEqualsPasswordConfirm = 21,
    WrongPassword = 22,
    
    // для ролей
    RoleAlreadyExists = 30,
    RoleNotFound = 31,
    UserAlreadyExistsThisRole = 32
}