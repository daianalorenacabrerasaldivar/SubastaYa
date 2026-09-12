using Application.Dto.Auctions;
using FluentValidation;

namespace Application.Validators
{
    public class CreateAuctionRequestValidator : AbstractValidator<CreateAuctionRequestDto>
    {
        public CreateAuctionRequestValidator()
        {
            RuleFor(x => x.VendedorId)
                .GreaterThan(0)
                .WithMessage("El Id del vendedor debe ser mayor que cero.");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0)
                .WithMessage("El Id de la categoría debe ser mayor que cero.");

            RuleFor(x => x.Titulo)
                .NotEmpty()
                .WithMessage("El título es obligatorio.")
                .MaximumLength(200)
                .WithMessage("El título no puede exceder 200 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres.");

            RuleFor(x => x.UrlImagen)
                .NotEmpty()
                .WithMessage("La URL de la imagen es obligatoria.")
                .MaximumLength(500)
                .WithMessage("La URL de la imagen no puede exceder 500 caracteres.");

            RuleFor(x => x.PrecioBase)
                .GreaterThan(0)
                .WithMessage("El precio base debe ser mayor que cero.");

            RuleFor(x => x.IncrementoMinimo)
                .GreaterThan(0)
                .WithMessage("El incremento mínimo debe ser mayor que cero.");

            RuleFor(x => x.FechaFin)
                .GreaterThan(x => x.FechaInicio)
                .WithMessage("La fecha de finalización debe ser posterior a la fecha de inicio.");
        }
    }
}
