using IdentityService.WebAPI.Login.Request;
using ImTools;
using Mypple_Music.Events;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using SixLabors.ImageSharp.Processing.Processors.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class LoginService : BaseService<SimpleUser>, ILoginService
    {
        private readonly HttpRestClient client;
        private readonly string ServiceName = "/IdentityService/api/Login";

        public LoginService(HttpRestClient client):base(client,"")
        {
            this.client = client;
        }

        public async Task<ApiResponse<SimpleUser?>> CreateUserWithPhoneNumAndCode(CreateUserWithPhoneNumAndCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/CreateUserWithPhoneNumAndCode";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<SimpleUser?>>(request);
            return res;
        }

        public async Task<SimpleUser> GetUserInfo()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{ServiceName}/GetUserInfo";
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<SimpleUser>(request);
            return res;
        }


        public async Task<ApiResponse<string?>> LoginByEmailAndPwd(LoginByEmailAndPwdRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/LoginByEmailAndPwd";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/LoginByPhoneAndPwd";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> LoginByUserNameAndPwd(LoginByUserNameAndPwdRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/LoginByUserNameAndPwd";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> SendCodeByPhone(SendCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/SendCodeByPhone";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> SendCodeByEmail(SendCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/SendCodeByEmail";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> LoginByPhoneAndCode(LoginByPhoneAndCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/LoginByPhoneAndCode";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> LoginByEmailAndCode(LoginByEmailAndCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/LoginByEmailAndCode";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> ChangePasswordWithCode(ChangePasswordWithCodeRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/ChangePasswordWithCode";
            request.Parameter = req;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<SimpleUser?>> UpdateUserInfo(UpdateUserInfoRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/UpdateUserInfo";
            request.Parameter = req;
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<ApiResponse<SimpleUser?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> ConfirmPhone(string phoneNumber)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/ConfirmPhone/?phoneNumber={phoneNumber}";
            request.Parameter = phoneNumber;
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> ConfirmEmail(string email)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/ConfirmEmail/?email={email}";
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> ChangeEmail(ChangePhoneOrEmailRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/ChangeEmail";
            request.Parameter = req;
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }

        public async Task<ApiResponse<string?>> ChangePhoneNum(ChangePhoneOrEmailRequest req)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"{ServiceName}/ChangePhoneNum";
            request.Parameter = req;
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<ApiResponse<string?>>(request);
            return res;
        }
    }
}
