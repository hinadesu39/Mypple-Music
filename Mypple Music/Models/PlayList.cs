using Mypple_Music.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class PlayList : BindableBase
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
                CreateLocalPicAsync(picUrl);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本地下载后歌曲名片的路径
        /// </summary>
        private string localPicUrl;
        public string LocalPicUrl
        {
            get { return localPicUrl; }
            set
            {
                localPicUrl = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 播放列表名
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
        /// 描述
        /// </summary>
        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged();
            }
        }

        async void CreateLocalPicAsync(Uri picUrl)
        {
            LocalPicUrl = await DownloadHelper.GetImageAsync(picUrl);
        }
    }
}
