using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.CreateBusiness.CompleteBusinessCreation;

public record CreateBusinessRequestDto(
    string FirstName,
    string LastName,
    string Title
);

public record CompleteBusinessCreationCommand(
    string Token,
    CreateBusinessRequestDto Dto
) : IRequest<Result<Unit, Error>>;