using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class PlayListService : BaseService<PlayList>, IPlayListService
    {
        private readonly HttpRestClient client;

        public PlayListService(HttpRestClient client)
            : base(client, "/Music.Main/api/PlayList")
        {
            this.client = client;
        }

        public async Task<Music[]> AddMusicToPlayListAsync(MusicAddToPlayListRequest request)
        {
            BaseRequest BaseRequest = new BaseRequest();
            BaseRequest.Method = RestSharp.Method.Post;
            BaseRequest.Route = $"/Music.Main/api/PlayList/AddMusicToPlayList";
            BaseRequest.Parameter = request;
            var res = await client.ExecuteAsync<Music[]>(BaseRequest);
            return res;
        }

        public async Task<PlayList> AddPlayListAsync(PlayListAddRequest request)
        {
            BaseRequest BaseRequest = new BaseRequest();
            BaseRequest.Method = RestSharp.Method.Post;
            BaseRequest.Route = $"/Music.Main/api/PlayList/Add";
            BaseRequest.Parameter = request;
            var res = await client.ExecuteAsync<PlayList>(BaseRequest);
            return res;
        }

        public async Task<Uri> UploadAsync(string url)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"/FileService/api/Uploader/Upload";
            request.Parameter = url;
            var res = await client.UploadAsync(request);
            return res;
        }
    }
}
