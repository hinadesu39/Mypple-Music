using Mypple_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public interface IMusicService:IBaseService<Music>
    {
        public Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId);
        public Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId);
        public Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId);
    }
}
