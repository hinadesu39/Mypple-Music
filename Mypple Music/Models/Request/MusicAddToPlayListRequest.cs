using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models.Request
{
    public class MusicAddToPlayListRequest
    {
        private Guid playListId;
        private Music[] musics;

        public Guid PlayListId
        {
            get => playListId;
            set => playListId = value;
        }
        public Music[] Musics
        {
            get => musics;
            set => musics = value;
        }

        public MusicAddToPlayListRequest(Guid playListId, Music[] musics)
        {
            this.playListId = playListId;
            this.musics = musics;
        }
    }
}
