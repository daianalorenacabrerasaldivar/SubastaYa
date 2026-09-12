using Application.Dto.Auctions;
using Domain.Common.ResultPattern;

namespace Application.Interfaces;

public interface ISubastaService
{
    Task<Result<AuctionDetailResponseDto>> CreateAsync(CreateAuctionRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<AuctionDetailResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
