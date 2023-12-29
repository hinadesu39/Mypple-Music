using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class MusicListViewModel : NavigationViewModel
    {
        #region Field
        private readonly IMusicService musicService;
        private readonly IPlayListService playListService;
        private ObservableCollection<Music> tempMusic;
        #endregion

        #region Property

        private ObservableCollection<PlayList> allPlayLists = AppSession.AllPlayLists;

        public ObservableCollection<PlayList> AllPlayLists
        {
            get { return allPlayLists; }
            set
            {
                allPlayLists = value;
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

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }
        public DelegateCommand<PlayList> AddToPlayListCommand { set; get; }
        #endregion

        #region Ctor
        public MusicListViewModel(IContainerProvider containerProvider, IMusicService musicService, IPlayListService playListService)
            : base(containerProvider)
        {
            this.musicService = musicService;
            this.playListService = playListService;
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            AddToPlayListCommand = new DelegateCommand<PlayList>(AddToPlayList);
            Config();
            
        }
        #endregion

        #region Command

        /// <summary>
        /// 添加单曲到播放列表
        /// </summary>
        /// <param name="list"></param>
        private async void AddToPlayList(PlayList list)
        {
            var musicAddToPlayListRequest = new MusicAddToPlayListRequest(list.Id, SelectedMusic);
            var res = await playListService.AddMusicToPlayListAsync(musicAddToPlayListRequest);
            if (res == null)
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}添加失败");
            }
            else
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}添加成功");
            }
        }

        private void TextEmpty()
        {
            if (tempMusic != null)
            {
                MusicList = tempMusic;
            }
        }

        private  void Search(string para)
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
            SelectedMusic = Music;
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
        }

        async void Config()
        {
            UpdateLoading(true);
            IsSearchVisible = false;
            var musics = await musicService.GetAllAsync();
            if (musics != null)
            {
                MusicList = new ObservableCollection<Music>(musics);
            }
            else
                eventAggregator.SendMessage("连接出现问题~~~");
            
            tempMusic = MusicList;
            UpdateLoading(false);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) { }
        #endregion
    }
}
