using IdentityService.WebAPI.Login.Request;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public interface ILoginService:IBaseService<SimpleUser>
    {
        Task<ApiResponse<string?>> LoginByEmailAndPwd(LoginByEmailAndPwdRequest req);
        Task<ApiResponse<string?>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req);
        Task<ApiResponse<string?>> LoginByUserNameAndPwd(LoginByUserNameAndPwdRequest req);
        Task<ApiResponse<string?>> LoginByPhoneAndCode(LoginByPhoneAndCodeRequest req);
        Task<ApiResponse<string?>> LoginByEmailAndCode(LoginByEmailAndCodeRequest req);
        Task<ApiResponse<SimpleUser?>> CreateUserWithPhoneNumAndCode(CreateUserWithPhoneNumAndCodeRequest req);
        Task<ApiResponse<string?>> SendCodeByPhone(SendCodeRequest req);
        Task<ApiResponse<string?>> SendCodeByEmail(SendCodeRequest req);
        Task<ApiResponse<string?>> ChangePasswordWithCode(ChangePasswordWithCodeRequest req);
        Task<SimpleUser> GetUserInfo();
        Task<ApiResponse<SimpleUser?>> UpdateUserInfo(UpdateUserInfoRequest req);
        Task<ApiResponse<string?>> ConfirmPhone(string phoneNumber);
        Task<ApiResponse<string?>> ConfirmEmail(string email);
        Task<ApiResponse<string?>> ChangeEmail(ChangePhoneOrEmailRequest req);
        Task<ApiResponse<string?>> ChangePhoneNum(ChangePhoneOrEmailRequest req);
    }
}
