
namespace IdentityService.WebAPI.Login.Request
{
    public record LoginByPhoneAndPwdRequest(string PhoneNum, string Password);
}
