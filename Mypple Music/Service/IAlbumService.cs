using Mypple_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public interface IAlbumService:IBaseService<Album>
    {
        public Task<Album[]> GetAlbumsByArtistIdAsync(Guid ArtistId);
        public Task<Album[]> GetAlbumsByMusicPostOrderAsync();
    }
}
