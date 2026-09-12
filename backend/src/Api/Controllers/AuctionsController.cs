using Application.Interfaces;
using Application.UseCases.Subasta.Command.Creacion;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using 

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/auctions")]
    public sealed class AuctionsController : ControllerBase
    {
        private readonly ISubastaService _subastaService;
        private readonly ILogger<AuctionsController> _logger;
        private readonly IMediator _mediator;

        public AuctionsController(ISubastaService subastaService, IMediator mediator, ILogger<AuctionsController> logger)
        {
            _subastaService = subastaService;
            _mediator = mediator;
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
        [ProducesResponseType(typeof(AuctionDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuctionDetailResponse>> CreateAuction(
           [FromBody] CreateAuctionCommand command,
           CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                switch (result.Status)
                {
                    case ResultPattern.DataStatus.Success:
                        // Created with location to GET by id
                        var created = result.Value;
                        return CreatedAtAction(nameof(GetAuctionById), new { id = created?.Id }, created);

                    case Domain.Common.ResultPattern.DataStatus.NotFound:
                        return NotFound(new ProblemDetails { Detail = result.Info });

                    case Domain.Common.ResultPattern.DataStatus.Conflict:
                        return Conflict(new ProblemDetails { Detail = result.Info });

                    case Domain.Common.ResultPattern.DataStatus.RequestValidation:
                        return BadRequest(new ProblemDetails { Detail = result.Info });

                    case Domain.Common.ResultPattern.DataStatus.NullOrEmpty:
                        return NoContent();

                    case Domain.Common.ResultPattern.DataStatus.Exception:
                        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Detail = result.Info });

                    case Domain.Common.ResultPattern.DataStatus.Failed:
                    default:
                        return BadRequest(new ProblemDetails { Detail = result.Info });
                }

            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors.GroupBy(x => x.PropertyName))
                {
                    ModelState.AddModelError(error.Key, string.Join(" ", error.Select(x => x.ErrorMessage)));
                }

                return ValidationProblem(ModelState);
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return ValidationProblem(ModelState);
            }
        }

        /// <summary>
        /// Obtiene el detalle de una subasta.
        /// </summary>
        /// <response code="200">Detalle de la subasta.</response>
        /// <response code="404">La subasta no existe.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AuctionDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuctionDetailResponse>> GetAuctionById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _subastaService.GetByIdAsync(id, cancellationToken);
            // Devolver según el DataStatus del Result
            switch (result.Status)
            {
                case Domain.Common.ResultPattern.DataStatus.Success:
                    return Ok(result.Value);

                case Domain.Common.ResultPattern.DataStatus.NotFound:
                    return NotFound(new ProblemDetails { Detail = result.Info });

                case Domain.Common.ResultPattern.DataStatus.Conflict:
                    return Conflict(new ProblemDetails { Detail = result.Info });

                case Domain.Common.ResultPattern.DataStatus.RequestValidation:
                    return BadRequest(new ProblemDetails { Detail = result.Info });

                case Domain.Common.ResultPattern.DataStatus.NullOrEmpty:
                    return NoContent();

                case Domain.Common.ResultPattern.DataStatus.Exception:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Detail = result.Info });

                case Domain.Common.ResultPattern.DataStatus.Failed:
                default:
                    return BadRequest(new ProblemDetails { Detail = result.Info });
            }
        }
    }
}
