using CryptoWallet.Api.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CryptoWallet.Api.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class ExceptionHandlerController : ControllerBase
{
    private readonly ILogger<ExceptionHandlerController> _logger;

    public ExceptionHandlerController(ILogger<ExceptionHandlerController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<RestApiError> HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerFeature?.Error == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new RestApiError
            {
                Title = "Unknown Error",
                Message = "Unexpected error occurred"
            });
        }

        var exception = exceptionHandlerFeature.Error;

        if (exception is BaseException baseException)
        {
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
            return StatusCode(baseException.StatusCode, new RestApiError
            {
                Title = baseException.Title,
                Message = baseException.Message
            });
        }


        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        return StatusCode(StatusCodes.Status500InternalServerError, new RestApiError
        {
            Title = "Internal Server Error",
            Message = exception.Message
        });
    }
}

