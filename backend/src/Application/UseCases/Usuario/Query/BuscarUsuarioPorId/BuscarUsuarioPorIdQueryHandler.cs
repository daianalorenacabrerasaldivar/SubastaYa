using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario.Query.BuscarUsuarioPorId
{
    public sealed class BuscarUsuarioPorIdQueryHandler
    : IRequestHandler<
        BuscarUsuarioPorIdQuery,
        Result<BuscarUsuarioPorIdResponse>>
    {
        private readonly IUsuarioQuery _usuarioQuery;

        public BuscarUsuarioPorIdQueryHandler(
            IUsuarioQuery usuarioQuery)
        {
            _usuarioQuery = usuarioQuery;
        }

        public async Task<Result<BuscarUsuarioPorIdResponse>> Handle(
            BuscarUsuarioPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _usuarioQuery.BuscarPorIdAsync(
                request.UsuarioId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return new Failed<BuscarUsuarioPorIdResponse>(
                    result.Info,
                    result.Status);
            }

            var usuario = result.Value;

            var response = new BuscarUsuarioPorIdResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                FechaRegistro = usuario.FechaRegistro
            };

            return new Success<BuscarUsuarioPorIdResponse>(response);
        }
    }

}
