using FluentValidation;

namespace Application.UseCases.Subastas.Query.Listar
{
    public sealed class ListAuctionsValidation : AbstractValidator<ListAuctionsQuery>
    {
        public const int MaxPageSize = 100;

        private const int MaxPage = int.MaxValue / MaxPageSize;

        public ListAuctionsValidation()
        {
            RuleFor(x => x.Page).InclusiveBetween(1, MaxPage).WithMessage("La página debe estar entre 1 y " + MaxPage + ".");
            RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize).WithMessage($"El tamaño de página debe estar entre 1 y {MaxPageSize}.");
            RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue).WithMessage("El Id de categoría debe ser mayor que cero.");
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue).WithMessage("El precio mínimo no puede ser negativo.");
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue).WithMessage("El precio máximo no puede ser negativo.");
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
                .WithMessage("El precio máximo debe ser mayor o igual al precio mínimo.");
        }
    }
}
