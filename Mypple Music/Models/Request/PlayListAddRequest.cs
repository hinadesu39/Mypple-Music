using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models.Request
{
    public class PlayListAddRequest
    {
        private Uri? picUrl;
        private string title;
        private string? description;

        public PlayListAddRequest(Uri? picUrl, string title, string? description)
        {
            this.picUrl = picUrl;
            this.title = title;
            this.description = description;
        }

        public Uri? PicUrl
        {
            get => picUrl;
            set => picUrl = value;
        }
        public string Title
        {
            get => title;
            set => title = value;
        }
        public string? Description
        {
            get => description;
            set => description = value;
        }
    }
}
