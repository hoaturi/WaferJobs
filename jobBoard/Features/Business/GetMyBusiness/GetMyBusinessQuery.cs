using JobBoard.Common.Models;
using JobBoard.Features.Business.GetBusiness;
using MediatR;

namespace JobBoard.Features.Business.GetMyBusiness;

public record GetMyBusinessQuery : IRequest<Result<GetBusinessResponse, Error>>;