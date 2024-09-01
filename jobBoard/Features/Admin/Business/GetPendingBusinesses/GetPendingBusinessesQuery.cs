using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Business.GetPendingBusinesses;

public record GetPendingBusinessesQuery : IRequest<Result<GetPendingBusinessesQueryResponse, Error>>;