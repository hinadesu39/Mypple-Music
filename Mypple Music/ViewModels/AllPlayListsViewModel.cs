using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class AllPlayListsViewModel : NavigationViewModel
    {
        #region Field
        private readonly IPlayListService playListService;
        private readonly IMusicService musicService;
        private readonly IRegionManager RegionManager;
        private readonly IDialogHostService dialog;
        private readonly IContainerProvider container;
        private IRegionNavigationJournal journal;
        private bool isUpdating;
        private ObservableCollection<PlayList> tempPlayList;
        #endregion

        #region Property
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

        private int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<PlayList> playList;

        public ObservableCollection<PlayList> AlbumList
        {
            get { return playList; }
            set
            {
                playList = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<PlayList> ConfirmAlbumCommand { get; set; }
        public DelegateCommand<PlayList> PlayAlbumCommand { get; set; }
        public DelegateCommand<PlayList> SettingAlbumCommand { get; set; }
        public DelegateCommand<PlayList> SelectedPlayListChangedCommand { get; set; }
        #endregion

        #region Ctor
        public AllPlayListsViewModel(
            IContainerProvider containerProvider,
            IPlayListService playListService,
            IMusicService musicService,
            IRegionManager RegionManager,
            IDialogHostService dialog
        )
            : base(containerProvider)
        {
            this.container = containerProvider;
            this.playListService = playListService;
            this.musicService = musicService;
            this.RegionManager = RegionManager;
            this.dialog = dialog;
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            ConfirmAlbumCommand = new(ConfirmPlayList);
            SelectedPlayListChangedCommand = new(SelectedPlayListChanged);
            PlayAlbumCommand = new(PlayPlayList);
            SettingAlbumCommand = new(SettingPlayList);
        }
        #endregion

        #region Command
        private void TextEmpty()
        {
            if (tempPlayList != null)
            {
                AlbumList = tempPlayList;
            }
        }

        private void Search(string para)
        {
            if (IsSearchVisible)
            {
                if (para == string.Empty)
                {
                    IsSearchVisible = false;
                    AlbumList = tempPlayList;
                    return;
                }
                //查找
                var searchedPlayListList = AlbumList.Where(a => a.Title.Contains(para,StringComparison.OrdinalIgnoreCase));
                if (searchedPlayListList != null)
                {
                    AlbumList = new ObservableCollection<PlayList>(searchedPlayListList);
                }
            }
            else
            {
                IsSearchVisible = true;
            }
        }

        private void SettingPlayList(PlayList PlayList)
        {
            Debug.WriteLine(PlayList);
        }

        private async void PlayPlayList(PlayList PlayList)
        {
            var MusicList = new ObservableCollection<Music>(
                await playListService.GetMusicsByPlayListIdAsync(PlayList.Id)
            );
            var music = MusicList.ToList()[0];
            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(new PlayListCreatedModel(MusicList, 0, "MainView"));
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(music, "LyricView"));
        }

        private void SelectedPlayListChanged(PlayList PlayList)
        {
            Debug.WriteLine(PlayList);
        }

        /// <summary>
        /// 跳转到具体的专辑展示界面
        /// </summary>
        private void ConfirmPlayList(PlayList PlayList)
        {
            if (isUpdating)
            {
                return;
            }
            NavigationParameters para = new NavigationParameters();
            para.Add("Id", PlayList.Id);
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                "PlayListView",
                Callback =>
                {
                    journal = Callback.Context.NavigationService.Journal;
                },
                para
            );
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //重置选中项
            isUpdating = true;
            SelectedIndex = -1;
            isUpdating = false;
            
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            UpdateLoading(true);
            var playLists = await playListService.GetAllAsync();
            if(playLists != null)
            {
                AlbumList = new ObservableCollection<PlayList>(playLists);
            }
            else
            {
                eventAggregator.SendMessage("连接出现问题~~~");
            }
            tempPlayList = AlbumList;
            UpdateLoading(false);
        }
        #endregion
    }
}
 