using System;

namespace BookStorePOS.Shared.Models.Common;

public enum EnumResType
{
    None = 0,
    Success = 1,
    ValidationError = 2,
    SystemError = 3,
    NotFound = 4,
    Conflict = 5,
    Unauthorized = 6
}

// Alias enum for ErrorType matching senior/user convention
public enum ErrorType
{
    None = 0,
    Success = 1,
    Validation = 2,
    ValidationError = 2,
    SystemError = 3,
    NotFound = 4,
    Conflict = 5,
    Unauthorized = 6
}

public class Result<T>
{
    public bool IsSuccess { get; set; }

    public bool IsError => !IsSuccess;

    public EnumResType Type { get; set; }

    public EnumResType ErrorType => Type;

    public string Message { get; set; } = string.Empty;

    public string ErrorMessage => Message;

    public T? Data { get; set; }

    public EnumResType GetEnumType() => Type;

    public static Result<T> Success(T? data, string message = "Success")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Type = EnumResType.Success,
            Data = data,
            Message = message
        };
    }

    public static Result<T> DeleteSuccess(string message = "Delete success")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Type = EnumResType.Success,
            Message = message
        };
    }

    public static Result<T> ValidationError(string message, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = EnumResType.ValidationError,
            Data = data,
            Message = message
        };
    }

    public static Result<T> SystemError(string message, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = EnumResType.SystemError,
            Data = data,
            Message = message
        };
    }

    public static Result<T> NotFound(string message, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = EnumResType.NotFound,
            Data = data,
            Message = message
        };
    }

    public static Result<T> Conflict(string message, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = EnumResType.Conflict,
            Data = data,
            Message = message
        };
    }

    public static Result<T> Unauthorized(string message, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = EnumResType.Unauthorized,
            Data = data,
            Message = message
        };
    }

    public static Result<T> Failure(string message, EnumResType type = EnumResType.ValidationError, T? data = default)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Type = type,
            Data = data,
            Message = message
        };
    }
}

public class Result : Result<object?>
{
    public static Result Ok(string message = "Success")
    {
        return new Result
        {
            IsSuccess = true,
            Type = EnumResType.Success,
            Message = message
        };
    }

    public static Result Fail(string message, EnumResType type = EnumResType.ValidationError)
    {
        return new Result
        {
            IsSuccess = false,
            Type = type,
            Message = message
        };
    }
}
