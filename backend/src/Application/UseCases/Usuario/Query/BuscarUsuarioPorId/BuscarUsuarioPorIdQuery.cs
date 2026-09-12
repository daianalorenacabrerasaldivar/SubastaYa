using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuario.Query.BuscarUsuarioPorId
{
    public record BuscarUsuarioPorIdQuery(int UsuarioId) : IRequest<Result<BuscarUsuarioPorIdResponse>>
    {
    }
}
