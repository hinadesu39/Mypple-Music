using MaterialDesignColors;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class MusicWithAlbumViewModel : NavigationViewModel
    {
        private bool isUpdating;
        private string whichAlbum;
        private IMusicService musicService;
        private readonly IDialogHostService dialog;
        private ObservableCollection<Music> tempMusic;

        /// <summary>
        /// PopUpButton弹起状态
        /// </summary>
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// PopUpButton内容选中下标
        /// </summary>
        private int popUpSelectedIndex = -1;

        public int PopUpSelectedIndex
        {
            get { return popUpSelectedIndex; }
            set
            {
                popUpSelectedIndex = value;
                RaisePropertyChanged();
            }
        }

        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set
            {
                isSearchVisible = value;
                RaisePropertyChanged();
            }
        }

        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                RaisePropertyChanged();
            }
        }

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

        private Album album;

        public Album Album
        {
            get { return album; }
            set
            {
                album = value;
                RaisePropertyChanged();
            }
        }

        private Music selectedMusic;

        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set
            {
                selectedMusic = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Music> musicList;

        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set
            {
                musicList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> popUpList;

        public ObservableCollection<string> PopUpList
        {
            get { return popUpList; }
            set
            {
                popUpList = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }
        public DelegateCommand<Music> PauseOrPlayCommand { set; get; }
        public DelegateCommand<string> NavigateCommand { get; set; }

        public MusicWithAlbumViewModel(
            IContainerProvider containerProvider,
            IMusicService musicService,
            IDialogHostService dialog
        )
            : base(containerProvider)
        {
            this.dialog = dialog;
            this.musicService = musicService;

            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            PauseOrPlayCommand = new DelegateCommand<Music>(PauseOrPlay);
            NavigateCommand = new DelegateCommand<string>(Navigation);
            var menu = new List<string>();
            menu.Add("下载全部歌曲");
            menu.Add("更多");
            menu.Add("属性");
            PopUpList = new ObservableCollection<string>(menu);
        }

        private async void Navigation(string obj)
        {
            if (isUpdating)
                return;
            IsChecked = false;
            switch (obj)
            {
                case "下载全部歌曲":
                    break;
                case "更多":
                    break;
                case "属性":
                    break;
            }

            isUpdating = true;
            PopUpSelectedIndex = -1;
            isUpdating = false;
        }

        private void PauseOrPlay(Music music)
        {
            if (SelectedMusic != music)
            {
                SelectedMusicChanged(music);
                return;
            }
            if (music.Status == Music.PlayStatus.PausePlay)
            {
                music.Status = Music.PlayStatus.StartPlay;
            }
            else if (music.Status == Music.PlayStatus.StartPlay)
            {
                music.Status = Music.PlayStatus.PausePlay;
            }
        }

        private void TextEmpty()
        {
            if (tempMusic != null)
            {
                MusicList = tempMusic;
            }
        }

        private void Search(string para)
        {
            if (IsSearchVisible)
            {
                if (para == string.Empty)
                {
                    IsSearchVisible = false;
                    MusicList = tempMusic;
                    return;
                }
                //查找
                var searchedMusicList = MusicList.Where(m => m.Title.Contains(para));
                if (searchedMusicList != null)
                {
                    MusicList = new ObservableCollection<Music>(searchedMusicList);
                }
            }
            else
            {
                IsSearchVisible = true;
            }
        }

        private void SelectedMusicChanged(Music Music)
        {
            if (isUpdating)
            {
                return;
            }
            isUpdating = true;
            //设置播放状态
            var playingMusic = MusicList.FirstOrDefault(
                m => m.Status == Music.PlayStatus.StartPlay
            );
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
            SelectedMusic = Music;
            SelectedMusic.Status = Music.PlayStatus.StartPlay;

            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(
                        MusicList,
                        MusicList.IndexOf(SelectedMusic),
                        "MainView"
                    )
                );
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Music, "LyricView"));
            isUpdating = false;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Album"))
            {
                //发现打开的是已加载的播放列表，则直接退出不必重新加载
                if (whichAlbum != null && Album.Title == whichAlbum)
                {
                    return;
                }
                Album = navigationContext.Parameters.GetValue<Album>("Album");
                AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(true));
                MusicList = new ObservableCollection<Music>(
                    await musicService.GetMusicsByAlbumIdAsync(Album.Id)
                );
                AppSession.EventAggregator
                    .GetEvent<LoadingEvent>()
                    .Publish(new LoadingModel(false));
                tempMusic = MusicList;
                Count = MusicList.Count;
                Duration = MusicList.Sum(m => m.Duration);
                whichAlbum = Album.Title;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
