

namespace IdentityService.WebAPI.Login.Request
{
    public record LoginByUserNameAndPwdRequest(string UserName, string Password);
}
