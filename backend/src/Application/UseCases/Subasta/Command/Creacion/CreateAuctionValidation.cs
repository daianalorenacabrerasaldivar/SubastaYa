using FluentValidation;

namespace Application.UseCases.Subasta.Command.Creacion
{
    public class CrearSubastaValidation : AbstractValidator<CreateAuctionCommand>
    {
        public CrearSubastaValidation()
        {
            RuleFor(x => x.VendedorId).GreaterThan(0).WithMessage("El Id del vendedor debe ser mayor que cero.");
            RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("El Id de la categoría debe ser mayor que cero.");
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El título no puede estar vacío.");
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripción no puede estar vacía.");
            RuleFor(x => x.UrlImagen)
                .NotEmpty()
                .Must(IsHttpUrl)
                .WithMessage("La URL de la imagen debe ser HTTP o HTTPS.");
            RuleFor(x => x.PrecioBase).GreaterThan(0).WithMessage("El precio base debe ser mayor que cero.");
            RuleFor(x => x.IncrementoMinimo).GreaterThan(0).WithMessage("El incremento mínimo debe ser mayor que cero.");
            RuleFor(x => x.FechaInicio)
                .NotEqual(default(DateTime))
                .WithMessage("La fecha de inicio es obligatoria.");
            RuleFor(x => x.FechaFin)
                .NotEqual(default(DateTime))
                .WithMessage("La fecha de finalización es obligatoria.");
            RuleFor(x => x.FechaFin)
                .GreaterThan(x => x.FechaInicio)
                .WithMessage("La fecha de finalización debe ser posterior a la fecha de inicio.");
        }

        private static bool IsHttpUrl(string? value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

    }
}
