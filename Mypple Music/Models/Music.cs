using Mypple_Music.Common;
using Mypple_Music.Events;
using Mypple_Music.ViewModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class Music : BindableBase
    {
        /// <summary>
        /// 歌曲id
        /// </summary>
        private Guid id;
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 歌曲名
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
        /// 歌曲url
        /// </summary>
        private Uri audioUrl;
        public Uri AudioUrl
        {
            get { return audioUrl; }
            set { audioUrl = value; }
        }

        ///// <summary>
        ///// 本地下载后的mp3路径
        ///// </summary>
        //private string localSongUrl;
        //public string LocalSongUrl
        //{
        //    get { return localSongUrl; }
        //    set
        //    {
        //        localSongUrl = value;
        //        RaisePropertyChanged();
        //    }
        //}

        /// <summary>
        /// 歌曲图片
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
        /// 歌曲时长
        /// </summary>

        private double duration;

        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 歌手
        /// </summary>
        ///
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
        /// 专辑
        /// </summary>
        private Guid albumId;
        public Guid AlbumId
        {
            get { return albumId; }
            set
            {
                albumId = value;
                RaisePropertyChanged();
            }
        }

        private string album;

        public string Album
        {
            get { return album; }
            set
            {
                album = value;
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
            set
            {
                type = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 歌词
        /// </summary>
        private string lyric;

        public string Lyric
        {
            get { return lyric; }
            set
            {
                lyric = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否喜欢
        /// </summary>
        private bool isLiked;

        public bool IsLiked
        {
            get { return isLiked; }
            set
            {
                isLiked = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 发行时间
        /// </summary>
        private int publishTime;

        public int PublishTime
        {
            get { return publishTime; }
            set
            {
                publishTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 播放次数
        /// </summary>
        private int playTimes;

        public int PlayTimes
        {
            get { return playTimes; }
            set
            {
                playTimes = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 播放状态
        /// </summary>
        private PlayStatus status = PlayStatus.StopPlay;


        public PlayStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                //通知播放状态的改变
                if (status != PlayStatus.StopPlay)
                    AppSession.EventAggregator
                       .GetEvent<MusicPlayStatusChangedEvent>()
                        .Publish(new MusicPlayStatusChangedModel(status, "MainView"));
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 播放状态
        /// </summary>
        public enum PlayStatus
        {
            StartPlay, //正在播放
            PausePlay, //暂停播放
            StopPlay //彻底关闭
        }

        async void CreateLocalPicAsync(Uri picUrl)
        {
            LocalPicUrl = await DownloadHelper.GetImageAsync(picUrl);
        }
    }
}
