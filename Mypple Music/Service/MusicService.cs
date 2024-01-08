using Mypple_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class MusicService : BaseService<Music>, IMusicService
    {
        private readonly HttpRestClient client;
        private readonly string ServiceName = "/Music.Main/api/Musics";
        public MusicService(HttpRestClient client)
            : base(client, "/Music.Main/api/Musics")
        {
            this.client = client;
        }

        public async Task<SearchedResult> GetByKeywordsAsync(string Keywords)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{ServiceName}/GetByKeyWords?keywords={Keywords}";
            var res = await client.ExecuteAsync<SearchedResult>(request);
            return res;
        }

        public async Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{ServiceName}/GetByAlbumId?albumId={AlbumId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }

        public async Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"{ServiceName}/GetByArtistId?artistId={ArtistId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }
    }
}
