﻿namespace JobBoard.Features.Business.ClaimBusiness.CompleteBusinessClaim;

public record CompleteBusinessClaimRequestDto(
    string FirstName,
    string LastName,
    string Title
);