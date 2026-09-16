using Application.UseCases.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/auth")]
    public sealed class AuthController : ApiControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Autentica un usuario y retorna un JWT Bearer.
        /// </summary>
        /// <remarks>
        /// Todos los usuarios del seed usan la contraseña <b>Password123!</b>
        /// Emails disponibles: vendedor@test.com, comprador1@test.com, comprador2@test.com, sinfondos@test.com
        /// </remarks>
        /// <param name="command">Email y contraseña del usuario.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">JWT token válido por 24 horas.</response>
        /// <response code="400">Email o contraseña vacíos.</response>
        /// <response code="404">Credenciales incorrectas.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return FromResult(result);
        }
    }
}
