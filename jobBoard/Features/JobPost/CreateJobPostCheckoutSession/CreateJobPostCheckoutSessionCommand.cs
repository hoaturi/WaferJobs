using JobBoard.Common.Models;
using JobBoard.Features.Payment;
using MediatR;

namespace JobBoard.Features.JobPost.CreateJobPostCheckoutSession;

public record CreateJobPostCheckoutSessionCommand(Guid Id)
    : IRequest<Result<CreateJobPostCheckoutSessionResponse, Error>>;