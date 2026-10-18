using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult ExecuteResult<T>(Result<T> result)
    {
        return result.Type switch
        {
            EnumResType.Success => Ok(result),
            EnumResType.NotFound => NotFound(result),
            EnumResType.ValidationError => BadRequest(result),
            EnumResType.Conflict => Conflict(result),
            EnumResType.Unauthorized => Unauthorized(result),
            EnumResType.SystemError => StatusCode(StatusCodes.Status500InternalServerError, result),
            _ => result.IsSuccess ? Ok(result) : BadRequest(result)
        };
    }

    protected IActionResult ExecuteResult(Result result)
    {
        return ExecuteResult<object?>(result);
    }
}
