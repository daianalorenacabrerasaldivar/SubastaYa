using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Subastas.Command.Adjudicar
{
    public sealed record AdjudicarSubastasVencidasCommand() : IRequest<Result<AdjudicacionResult>>;
}
