using System.Security.AccessControl;

namespace FonTech.Domain.Result;

public class BaseResult
{
    public bool IsSuccess => ErrorMessage == null;

    public string ErrorMessage { get; init; }
    
    // custom error codes
    public int? ErrorCode { get; set; }
}

public class BaseResult<T> : BaseResult
{
    public T Data { get; set; }

    public BaseResult(string errorMessage, int errorCode, T data)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Data = data;
    }

    public BaseResult(){ }

    public BaseResult(T data)
    {
        Data = data;
    }
}