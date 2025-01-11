namespace FonTech.Domain.Enum;

public enum ErrorCodes
{
    // 0 - 10 - Business Entities
    // 11 - 20 - User
    ReportsNotFound = 0,
    ReportNotFound = 1,
    ReportAlreadyExists = 2,
    
    UserNotFound = 11,
    
    InternalServerError = 10,
    
}