using Application.UseCases.Subasta.Command.Creacion;
using Domain.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/auctions")]
    public sealed class AuctionsController : ControllerBase
    {
        private readonly ILogger<AuctionsController> _logger;
        private readonly ISender _Sender;

        public AuctionsController(ISender sender, ILogger<AuctionsController> logger)
        {
            _Sender = sender;
            _logger = logger;
        }

        /// <summary>
        /// Crea y publica una subasta como vendedor.
        /// </summary>
        /// <remarks>
        /// Una fecha de inicio futura crea la subasta como PROGRAMADA; en otro caso se crea como ACTIVA.
        /// </remarks>
        /// <response code="201">La subasta fue creada correctamente.</response>
        /// <response code="400">Los datos enviados no cumplen las validaciones.</response>
        /// <response code="404">El vendedor o la categoría no existen.</response>
        [HttpPost]
        [ProducesResponseType(
    typeof(AuctionDetailResponse),
    StatusCodes.Status201Created)]
        [ProducesResponseType(
    typeof(ValidationProblemDetails),
    StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuctionDetailResponse>> CreateAuction(
    [FromBody] CreateAuctionCommand command,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _Sender.Send(
                    command,
                    cancellationToken);

                if (result.IsSuccess)
                {
                    return StatusCode(
                        StatusCodes.Status201Created,
                        result.Value);
                }

                return result.Status switch
                {
                    DataStatus.RequestValidation =>
                        BadRequest(new ValidationProblemDetails
                        {
                            Title = "Error de validación",
                            Detail = result.Info,
                            Status = StatusCodes.Status400BadRequest
                        }),

                    DataStatus.NotFound =>
                        NotFound(new ProblemDetails
                        {
                            Title = "Recurso no encontrado",
                            Detail = result.Info,
                            Status = StatusCodes.Status404NotFound
                        }),

                    DataStatus.Conflict =>
                        Conflict(new ProblemDetails
                        {
                            Title = "Conflicto",
                            Detail = result.Info,
                            Status = StatusCodes.Status409Conflict
                        }),

                    DataStatus.Exception =>
                        StatusCode(
                            StatusCodes.Status500InternalServerError,
                            new ProblemDetails
                            {
                                Title = "Error interno",
                                Detail = result.Info,
                                Status = StatusCodes.Status500InternalServerError
                            }),

                    _ =>
                        BadRequest(new ProblemDetails
                        {
                            Title = "No se pudo crear la subasta",
                            Detail = result.Info,
                            Status = StatusCodes.Status400BadRequest
                        })
                };
            }
            catch (OperationCanceledException)
            {
                //cuand se cancela el CancellationToken, se lanza esta excepción. Se puede manejar para devolver un 499 o simplemente relanzarla.
                //NGINX detecta cliente desconectado responde 499 pero no hay un standard HTTP para esto. Se puede usar 408 Request Timeout, pero no es exactamente lo mismo.
                throw;
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Title = "Error interno del servidor",
                        Detail = "Ocurrió un error inesperado.",
                        Status = StatusCodes.Status500InternalServerError
                    });
            }

        }
    }



}
