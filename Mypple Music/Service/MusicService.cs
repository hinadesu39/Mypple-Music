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
    }
}
