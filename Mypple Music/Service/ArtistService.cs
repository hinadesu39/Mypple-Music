using Mypple_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class ArtistService : BaseService<Artist>, IArtistService
    {
        private readonly HttpRestClient client;

        public ArtistService(HttpRestClient client)
            : base(client, "/Music.Main/api/Artists")
        {
            this.client = client;
        }
    }
}
