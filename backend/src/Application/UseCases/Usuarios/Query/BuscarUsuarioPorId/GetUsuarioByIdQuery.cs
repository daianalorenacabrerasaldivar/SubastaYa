using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorId
{
    public record GetUsuarioByIdQuery(int UsuarioId) : IRequest<Result<GetUsuarioByIdResponse>>
    {
    }
}
