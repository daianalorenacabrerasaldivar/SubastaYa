using Application.Common;
using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Subastas.Query.Listar
{
    public sealed class ListAuctionsHandler : IRequestHandler<ListAuctionsQuery, Result<PagedResponse<AuctionListItem>>>
    {
        private readonly ISubastaQueryRepository _subastaRepository;
        private readonly IValidator<ListAuctionsQuery> _validator;

        public ListAuctionsHandler(ISubastaQueryRepository subastaRepository, IValidator<ListAuctionsQuery> validator)
        {
            _subastaRepository = subastaRepository;
            _validator = validator;
        }

        public async Task<Result<PagedResponse<AuctionListItem>>> Handle(ListAuctionsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
                return new Failed<PagedResponse<AuctionListItem>>(errors, DataStatus.RequestValidation);
            }

            var filter = new AuctionListFilter(
                request.Status,
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.Page,
                request.PageSize);

            var page = await _subastaRepository.ListAsync(filter, cancellationToken);

            var response = PagedResponse<AuctionListItem>.Create(
                page.Items,
                request.Page,
                request.PageSize,
                page.TotalItems,
                DateTime.UtcNow);

            return new Success<PagedResponse<AuctionListItem>>(response);
        }
    }
}
