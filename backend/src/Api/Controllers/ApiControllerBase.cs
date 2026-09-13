using Domain.Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult FromResult<T>(Result<T> result, int successStatus = StatusCodes.Status200OK)
        {
            if (result.IsSuccess)
            {
                return StatusCode(successStatus, result.Value);
            }

            var (status, title) = result.Status switch
            {
                DataStatus.RequestValidation => (StatusCodes.Status400BadRequest, "Error de validación"),
                DataStatus.NotFound => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                DataStatus.Conflict => (StatusCodes.Status409Conflict, "Conflicto"),
                DataStatus.BusinessRule => (StatusCodes.Status422UnprocessableEntity, "Regla de negocio no cumplida"),
                DataStatus.Exception => (StatusCodes.Status500InternalServerError, "Error interno"),
                _ => (StatusCodes.Status400BadRequest, "La operación no pudo completarse")
            };

            return StatusCode(status, new ProblemDetails
            {
                Title = title,
                Detail = result.Info,
                Status = status
            });
        }
    }
}
