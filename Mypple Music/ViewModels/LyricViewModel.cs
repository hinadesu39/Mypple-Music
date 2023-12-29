using Mypple_Music.Common;
using Mypple_Music.Events;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Mypple_Music.ViewModels
{
    public class LyricViewModel : NavigationViewModel
    {
        #region Field
        public static bool IsAlive;
        private static PeriodicTimer Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        private MediaElement mediaElement;
        private readonly ILyricService lyricService;
        #endregion

        #region Property
        private string blurBackground;

        public string BlurBackground
        {
            get { return blurBackground; }
            set
            {
                blurBackground = value;
                RaisePropertyChanged();
            }
        }

        private Music music;

        public Music Music
        {
            get { return music; }
            set
            {
                music = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Lyric> lyrics;

        public ObservableCollection<Lyric> Lyrics
        {
            get { return lyrics; }
            set
            {
                lyrics = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Music> playList;

        public ObservableCollection<Music> PlayList
        {
            get { return playList; }
            set
            {
                playList = value;
                RaisePropertyChanged();
            }
        }

        private int selectedLyricIndex;

        public int SelectedLyricIndex
        {
            get { return selectedLyricIndex; }
            set
            {
                selectedLyricIndex = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ClickSentenceCommand { set; get; }
        #endregion

        #region Ctor
        public LyricViewModel(IContainerProvider containerProvider, ILyricService lyricService)
            : base(containerProvider)
        {
            this.lyricService = lyricService;
            ClickSentenceCommand = new DelegateCommand(ClickSentence);

            //事件订阅
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Subscribe(
                    async arg =>
                    {
                        Music = arg.music;
                        Lyrics = this.lyricService.LyricSplitter(Music.Lyric);
                        BlurBackground = await DownloadHelper.GetGaussianBlurImageAsync(
                            Music.PicUrl
                        );
                    },
                    m =>
                    {
                        return m.filter == "LyricView";
                    }
                );
        }
        #endregion

        #region Command
        /// <summary>
        /// 点击歌词跳转对应播放部分
        /// </summary>
        private void ClickSentence()
        {
            var clickedLyric = Lyrics[SelectedLyricIndex];
            mediaElement.Position = TimeSpan.FromSeconds(clickedLyric.TimeSpan);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (IsAlive)
            {
                return;
            }
            IsAlive = true;
            //获取歌词信息以及当前播放列表，并设置歌词滚动
            if (navigationContext.Parameters.ContainsKey("LyricInfo"))
            {
                var lyricInfo = navigationContext.Parameters.GetValue<LyricCreatedModel>(
                    "LyricInfo"
                );
                Music = lyricInfo.music;
                Lyrics = lyricService.LyricSplitter(Music.Lyric);
                BlurBackground = await DownloadHelper.GetGaussianBlurImageAsync(Music.PicUrl);
                mediaElement = lyricInfo.MediaElement;
                //设置滚动
                while (await Timer.WaitForNextTickAsync())
                {
                    //播放进度
                    var totalSeconds = mediaElement.Position.TotalSeconds;

                    //歌词滚动
                    var tempLyric = Lyrics.FirstOrDefault(t => t.TimeSpan >= totalSeconds + 1);
                    if (tempLyric != null)
                    {
                        SelectedLyricIndex = Lyrics.IndexOf(tempLyric) - 1;
                    }
                    else
                    {
                        SelectedLyricIndex = Lyrics.Count - 1;
                    }
                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //IsAlive = false;
        }
        #endregion
    }
}
