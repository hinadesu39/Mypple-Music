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

        public MusicService(HttpRestClient client)
            : base(client, "/Music.Main/api/Musics")
        {
            this.client = client;
        }

        public async Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"/Music.Main/api/Musics/GetByAlbumId?albumId={AlbumId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }

        public async Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"/Music.Main/api/Musics/GetByArtistId?artistId={ArtistId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }

        public async Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"/Music.Main/api/Musics/GetByPlayListId?playListId={PlayListId}";
            var res = await client.ExecuteAsync<Music[]>(request);
            return res;
        }
    }
}
