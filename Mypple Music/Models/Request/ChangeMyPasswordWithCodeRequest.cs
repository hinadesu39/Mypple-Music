

namespace IdentityService.WebAPI.Login.Request
{
    public record ChangePasswordWithCodeRequest(string account, string code, string Password, string Password2);


}
