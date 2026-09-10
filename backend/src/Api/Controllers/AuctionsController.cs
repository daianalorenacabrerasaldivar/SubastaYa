using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController :   ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuctionsController> _logger;
        public AuctionsController(IMediator mediator, ILogger<AuctionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuctionSummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult<AuctionSummaryDto>> CreateAuction(
           [FromBody] CreateAuctionDto createDto,
           CancellationToken cancellationToken = default)
        {
        }

        }
}
