using Mypple_Music.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;
        private readonly ILogger logger;
        public HttpRestClient(string apiUrl, ILogger logger)
        {
            this.apiUrl = apiUrl;
            this.logger = logger;
            client = new RestClient(apiUrl);
        }

        public async Task<Uri> UploadAsync(BaseRequest baseRequest)
        {
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url, baseRequest.Method);
            request.AlwaysMultipartFormData = true;
            if (baseRequest.Parameter != null)
            {
                request.AddFile("File", baseRequest.Parameter.ToString());
            }
            //request.AddHeader("Authorization", "Bearer " + token);//增加的 JWT 认证
            try
            {
                RestResponse response = await client.ExecuteAsync(request);
                return JsonConvert.DeserializeObject<Uri>(response.Content);
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
                return null;
            }

        }

        public async Task<T> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                request.AddJsonBody(baseRequest.Parameter);
            }
            if (baseRequest.Authorization != null)
            {
                request.AddHeader("Authorization", $"Bearer {baseRequest.Authorization}");
            }
            try
            {
                RestResponse response = await client.ExecuteAsync(request);
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return default(T);
            }


        }

    }
}
