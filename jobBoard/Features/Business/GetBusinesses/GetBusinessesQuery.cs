using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.GetBusinesses;

public record GetBusinessesQuery : IRequest<Result<GetBusinessesResponse, Error>>;