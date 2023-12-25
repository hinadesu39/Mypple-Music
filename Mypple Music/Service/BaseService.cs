using Mypple_Music.Events;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity>
        where TEntity : class
    {
        private readonly HttpRestClient client;
        protected readonly string serviceName;

        public BaseService(HttpRestClient client, string ServiceName)
        {
            this.client = client;
            serviceName = ServiceName;
        }

        public Task<TEntity> DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{serviceName}/GetAll";
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<List<TEntity>>(request);
            return res;
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{serviceName}/GetById?id={id}";
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<TEntity>(request);
            return res;
        }

        public async Task<List<TEntity>> GetByNameAsync(string name)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{serviceName}/GetByName?name={name}";
            request.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<List<TEntity>>(request);

            return res;
        }

        public async Task<string?> UploadAsync(string url)
        {
            var client = new RestClient();
            var request = new RestRequest("http://localhost/FileService/api/UpLoader/Upload", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddFile("file", url);
            RestResponse response = await client.ExecuteAsync(request);
            return response.Content.Replace("\"", "");
        }
    }
}
