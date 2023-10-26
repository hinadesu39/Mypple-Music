using Mypple_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class AlbumService : BaseService<Album>, IAlbumService
    {
        private readonly HttpRestClient client;

        public AlbumService(HttpRestClient client)
            : base(client, "/Music.Main/api/Albums")
        {
            this.client = client;
        }
    }
}
