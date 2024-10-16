namespace WaferJobs.Features.Business.CreateBusiness.CompleteBusinessCreation;

public record CreateBusinessRequestDto(
    string FirstName,
    string LastName,
    string Title
);