using Core.Dtos.Authorize;
using Core.Dtos.User;

namespace Infrastructure.Services.Authorize;

public interface IAuthenticateService
{
    bool IsAuthenticated(LoginUserDto user, out string token, out UserInfoDto userData, out string failReason);
}