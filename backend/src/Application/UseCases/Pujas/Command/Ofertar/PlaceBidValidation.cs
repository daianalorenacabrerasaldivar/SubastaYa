using FluentValidation;

namespace Application.UseCases.Pujas.Command.Ofertar
{
    public sealed class PlaceBidValidation : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidValidation()
        {
            RuleFor(x => x.SubastaId).GreaterThan(0).WithMessage("El Id de la subasta debe ser mayor que cero.");
            RuleFor(x => x.CompradorId).GreaterThan(0).WithMessage("El Id del comprador debe ser mayor que cero.");
            RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");
        }
    }
}
