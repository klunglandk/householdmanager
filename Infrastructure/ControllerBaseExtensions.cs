using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Infrastructure
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult IdentityProblem(this ControllerBase controller, IdentityResult result)
        {
            var errors = result.Errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            return controller.ValidationProblem(new ValidationProblemDetails(errors));
        }

        public static ActionResult BadRequestProblem(this ControllerBase controller, string detail)
        {
            return controller.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest, title: "Validation failed");
        }
    }
}
