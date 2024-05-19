namespace JobBoard.Domain.Auth;

public enum RefreshTokenStatus
{
    Valid,
    Revoked,
    Expired
}