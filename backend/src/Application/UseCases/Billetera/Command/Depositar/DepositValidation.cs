using FluentValidation;

namespace Application.UseCases.Billetera.Command.Depositar
{
    public sealed class DepositValidation : AbstractValidator<DepositCommand>
    {
        public DepositValidation()
        {
            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("El Id del usuario debe ser mayor que cero.");

            RuleFor(x => x.Monto)
                .GreaterThan(0).WithMessage("El monto del deposito debe ser mayor que cero.")
                .LessThanOrEqualTo(1_000_000).WithMessage("El monto maximo por deposito es $1.000.000.");
        }
    }
}
