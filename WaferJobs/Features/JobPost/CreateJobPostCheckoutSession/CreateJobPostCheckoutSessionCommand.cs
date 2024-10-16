using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Features.Payment;

namespace WaferJobs.Features.JobPost.CreateJobPostCheckoutSession;

public record CreateJobPostCheckoutSessionCommand(Guid Id)
    : IRequest<Result<CreateJobPostCheckoutSessionResponse, Error>>;