using Application.Services;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case InvalidCategoryNameException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Regra de Negócio violada";
                problemDetails.Detail = exception.Message;
                break;

            case EntityNotFoundInfraException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Recurso não encontrado";
                problemDetails.Detail = exception.Message;
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Erro interno no servidor";
                problemDetails.Detail = $"Ocorreu um erro inesperado: {exception.Message}";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}