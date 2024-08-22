using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Conference.GetConferences;

public record GetConferencesQuery : IRequest<Result<GetConferencesResponse, Error>>;