using Mypple_Music.Events;
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
            BaseRequest.Authorization = AppSession.JWTToken;
            var res = await client.ExecuteAsync<PlayList>(BaseRequest);
            return res;
        }

        public async Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{serviceName}/GetByPlayListId?playListId={PlayListId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }
    }
}
