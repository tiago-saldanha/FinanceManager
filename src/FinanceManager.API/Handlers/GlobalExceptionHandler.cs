using Application.Exceptions;
using Domain.Exceptions;
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

        var message = exception.InnerException != null ? $"{exception.Message}: {exception.InnerException.Message}" : $"{exception.Message}";

        switch (exception)
        {
            case CategoryNameAppException:
            case TransactionTypeAppException:
            case TransactionStatusAppException:
            case EntityAlreadyExistsInfraException:
            case TransactionPayException:
            case TransactionReopenException:
            case TransactionCancelException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Regra de Negócio violada";
                problemDetails.Detail = message;
                break;
            case EntityNotFoundInfraException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Recurso não encontrado";
                problemDetails.Detail = message;
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Erro interno no servidor";
                problemDetails.Detail = $"Ocorreu um erro inesperado: {message}";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}