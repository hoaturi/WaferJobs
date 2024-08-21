using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Business.GetClaimList;

public record GetClaimListQuery(string? Status) : IRequest<Result<GetClaimListResponse, Error>>;