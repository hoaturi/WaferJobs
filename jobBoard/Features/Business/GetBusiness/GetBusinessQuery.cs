using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.GetBusiness;

public record GetBusinessQuery(string Slug) : IRequest<Result<GetBusinessResponse, Error>>;