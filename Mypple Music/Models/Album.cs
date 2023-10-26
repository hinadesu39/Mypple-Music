using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class Album : BindableBase
    {
        /// <summary>
        /// 专辑id
        /// </summary>
        private Guid id;
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 专辑图片
        /// </summary>
        private Uri? picUrl;
        public Uri? PicUrl
        {
            get { return picUrl; }
            set
            {
                picUrl = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 专辑名
        /// </summary>
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 歌手
        /// </summary>
        private Guid artistId;

        public Guid ArtistId
        {
            get { return artistId; }
            set
            {
                artistId = value;
                RaisePropertyChanged();
            }
        }
        private string artist;

        public string Artist
        {
            get { return artist; }
            set
            {
                artist = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// 发行年份
        /// </summary>
        private int publishTime;

        public int PublishTime
        {
            get { return publishTime; }
            set { publishTime = value; }
        }
    }
}
