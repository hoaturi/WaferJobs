using JobBoard.Domain.Auth;

namespace JobBoard.Domain.Business;

public class BusinessUserEntity : ApplicationUserEntity
{
    public BusinessEntity? Business { get; set; }
}