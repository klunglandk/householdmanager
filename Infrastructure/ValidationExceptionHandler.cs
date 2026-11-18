using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Infrastructure
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        private readonly IProblemDetailsService _problemDetailsService;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ValidationException validationException) return false;

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = validationException.Message
                }
            });
        }
    }
}
