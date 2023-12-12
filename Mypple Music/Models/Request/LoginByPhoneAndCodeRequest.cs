

namespace IdentityService.WebAPI.Login.Request
{
    public record LoginByPhoneAndCodeRequest(string PhoneNum, string Code);

}
