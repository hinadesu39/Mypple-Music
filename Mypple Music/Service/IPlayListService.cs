using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public interface IPlayListService:IBaseService<PlayList>
    {
        public Task<PlayList> AddPlayListAsync(PlayListAddRequest request);
        public Task<Music[]> AddMusicToPlayListAsync(MusicAddToPlayListRequest request);
        public Task<Uri> UploadAsync(string url);
        public Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId);
    }
}
