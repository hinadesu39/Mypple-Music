using ImTools;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Mypple_Music.ViewModels
{
    public class LyricViewModel : NavigationViewModel
    {
        public static bool IsAlive;
        private static PeriodicTimer Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        private MediaElement mediaElement;

        private ILyricService lyricService;
        private int musicIndex;

        private string blurBackground;

        public string BlurBackground
        {
            get { return blurBackground; }
            set { blurBackground = value; RaisePropertyChanged(); }
        }


        private Music music;

        public Music Music
        {
            get { return music; }
            set { music = value; RaisePropertyChanged(); }
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

        private List<Music> playList;

        public List<Music> PlayList
        {
            get { return playList; }
            set { playList = value; }
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
       
        public LyricViewModel(IContainerProvider containerProvider, ILyricService lyricService)
            : base(containerProvider)
        {                  
            this.lyricService = lyricService;
            ClickSentenceCommand = new DelegateCommand(ClickSentence);

            //事件订阅
            eventAggregator.GetEvent<MusicPlayedEvent>().Subscribe(async arg =>
            {
               
                Music = arg.music;
                Lyrics = this.lyricService.LyricSplitter(Music.Lyric);
                BlurBackground =Path.GetFullPath(await FileHelper.GetGaussianBlurImageAsync(Music.PicUrl));
            },
            m =>
            {
                return m.filter == "LyricView";
            });
        }

       
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
            //base.OnNavigatedTo(navigationContext);
            //获取歌词信息以及当前播放列表，并设置歌词滚动
            if (navigationContext.Parameters.ContainsKey("LyricInfo"))
            {
                var lyricInfo = navigationContext.Parameters.GetValue<LyricCreatedModel>(
                    "LyricInfo"
                );
                PlayList = lyricInfo.PlayList;
                Music = lyricInfo.PlayList[lyricInfo.PlayIndex];
                mediaElement = lyricInfo.MediaElement;
                Lyrics = lyricService.LyricSplitter(Music.Lyric);
                BlurBackground = Path.GetFullPath(await FileHelper.GetGaussianBlurImageAsync(Music.PicUrl));


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
    }
}
