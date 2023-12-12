namespace IdentityService.WebAPI.Login.Request
{
    public record CreateUserWithPhoneNumAndCodeRequest(string userName, string PhoneNum, string passWord, string code);
}
