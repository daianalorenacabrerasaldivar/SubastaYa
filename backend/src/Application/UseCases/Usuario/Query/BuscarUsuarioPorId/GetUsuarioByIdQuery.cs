using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuario.Query.BuscarUsuarioPorId
{
    public record GetUsuarioByIdQuery(int UsuarioId) : IRequest<Result<GetUsuarioByIdResponse>>
    {
    }
}
