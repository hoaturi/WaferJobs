using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.GetBusiness;

public record GetBusinessQuery(Guid Id) : IRequest<Result<GetBusinessResponse, Error>>;