using MaterialDesignColors;
using Mypple_Music.Common;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TagLib.Id3v2;

namespace Mypple_Music.ViewModels
{
    public class PlayListViewModel : NavigationViewModel
    {
        private bool isUpdating;
        private Guid whichPlayList;
        private IMusicService musicService;
        private readonly IDialogHostService dialog;
        private IPlayListService playListService;
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<Music> tempMusic;
        private Guid playListId;
        private Music musicToEdit;


        /// <summary>
        /// 下载是否进行
        /// </summary>
        private bool isDownloading;

        public bool IsDownloading
        {
            get { return isDownloading; }
            set { isDownloading = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// PopUpButton弹起状态
        /// </summary>
        private bool isMainChecked;

        public bool IsMainChecked
        {
            get { return isMainChecked; }
            set
            {
                isMainChecked = value;
                RaisePropertyChanged();
            }
        }

        private bool isSubChecked;

        public bool IsSubChecked
        {
            get { return isSubChecked; }
            set
            {
                isSubChecked = value;
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

        private PlayList playList;

        public PlayList PlayList
        {
            get { return playList; }
            set
            {
                playList = value;
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

        private ObservableCollection<string> mainPopUpList;

        public ObservableCollection<string> MainPopUpList
        {
            get { return mainPopUpList; }
            set
            {
                mainPopUpList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> subPopUpList;

        public ObservableCollection<string> SubPopUpList
        {
            get { return subPopUpList; }
            set
            {
                subPopUpList = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }
        public DelegateCommand<Music> PauseOrPlayCommand { set; get; }
        public DelegateCommand<string> NavigateCommand { get; set; }
        public DelegateCommand<Music> MusicToFocusCommand { get; set; }
        public DelegateCommand DownloadAllCommand { set; get; }

        public PlayListViewModel(
            IContainerProvider containerProvider,
            IMusicService musicService,
            IDialogHostService dialog,
            IPlayListService playListService,
            IEventAggregator eventAggregator
        )
            : base(containerProvider)
        {
            this.dialog = dialog;
            this.musicService = musicService;
            this.playListService = playListService;
            this.eventAggregator = eventAggregator;

            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            PauseOrPlayCommand = new DelegateCommand<Music>(PauseOrPlay);
            NavigateCommand = new DelegateCommand<string>(Navigation);
            MusicToFocusCommand = new DelegateCommand<Music>(MusicToFocus);
            DownloadAllCommand = new DelegateCommand(DownloadAllAsync);
            Config();
        }

        private async void DownloadAllAsync()
        {
            IsDownloading = true;
            foreach (var m in MusicList)
            {
                try
                {
                    var res = await DownloadHelper.GetMusicAsync(m.AudioUrl);
                    if (res != null)
                    {
                        eventAggregator.SendMessage($"{m.Title} 下载成功！");
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    eventAggregator.SendMessage($"{m.Title} 下载失败,请稍后再试！");
                }
            }
            IsDownloading = false;
        }

        private void MusicToFocus(Music music)
        {
            musicToEdit = music;
        }

        private void Config()
        {
            var mainMenu = new List<string>();
            mainMenu.Add("添加歌曲");
            mainMenu.Add("更多");
            mainMenu.Add("属性");
            MainPopUpList = new ObservableCollection<string>(mainMenu);

            var subMenu = new List<string>();
            subMenu.Add("下载该歌曲");
            subMenu.Add("从播放列表中移除");
            subMenu.Add("属性");
            SubPopUpList = new ObservableCollection<string>(subMenu);

        }

        private async void Navigation(string obj)
        {
            if (isUpdating)
                return;
            IsMainChecked = false;
            DialogParameters parameter = new DialogParameters();
            switch (obj)
            {
                case "添加歌曲":
                    parameter.Add("Id", playListId);
                    var dialogRes = await dialog.ShowDialog("AddMusicView", parameter);
                    if (dialogRes.Result == ButtonResult.OK)
                    {
                        var musicAddToPlayListRequest =
                            dialogRes.Parameters.GetValue<MusicAddToPlayListRequest>("Value");
                        var res = await playListService.AddMusicToPlayListAsync(
                            musicAddToPlayListRequest
                        );
                        MusicList.AddRange(res);
                        tempMusic = MusicList;
                    }
                    break;
                case "删除该播放列表":
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


            if (navigationContext.Parameters.ContainsKey("Id"))
            {
                playListId = navigationContext.Parameters.GetValue<Guid>("Id");
                //发现打开的是已加载的播放列表，则直接退出不必重新加载
                if (whichPlayList != Guid.Empty && PlayList.Id == whichPlayList)
                {
                    return;
                }
                AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(true));
                PlayList = await playListService.GetByIdAsync(playListId);
                MusicList = new ObservableCollection<Music>(
                    await musicService.GetMusicsByPlayListIdAsync(playListId)
                );
                AppSession.EventAggregator
                    .GetEvent<LoadingEvent>()
                    .Publish(new LoadingModel(false));
                tempMusic = MusicList;
                Count = MusicList.Count;
                Duration = MusicList.Sum(m => m.Duration);
                if (whichPlayList == Guid.Empty)
                    whichPlayList = PlayList.Id;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
