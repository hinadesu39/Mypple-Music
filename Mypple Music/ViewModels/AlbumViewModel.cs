using ImTools;
using MaterialDesignColors;
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
    public class AlbumViewModel : NavigationViewModel
    {
        private IAlbumService albumService;
        private IMusicService musicService;
        private readonly IRegionManager RegionManager;
        private readonly IDialogHostService dialog;
        private readonly IContainerProvider container;
        private IRegionNavigationJournal journal;
        private bool isUpdating;
        private ObservableCollection<Album> tempAlbum;

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
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

        private ObservableCollection<Album> albumList;

        public ObservableCollection<Album> AlbumList
        {
            get { return albumList; }
            set
            {
                albumList = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Album> ConfirmAlbumCommand { get; set; }
        public DelegateCommand<Album> PlayAlbumCommand { get; set; }
        public DelegateCommand<Album> SettingAlbumCommand { get; set; }
        public DelegateCommand<Album> SelectedAlbumChangedCommand { get; set; }

        public AlbumViewModel(
            IContainerProvider containerProvider,
            IAlbumService albumService,
            IMusicService musicService,
            IRegionManager RegionManager,
            IDialogHostService dialog
        )
            : base(containerProvider)
        {
            this.container = containerProvider;
            this.albumService = albumService;
            this.musicService = musicService;
            this.RegionManager = RegionManager;
            this.dialog = dialog;
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            ConfirmAlbumCommand = new(ConfirmAlbum);
            SelectedAlbumChangedCommand = new(SelectedAlbumChanged);
            PlayAlbumCommand = new(PlayAlbum);
            SettingAlbumCommand = new(SettingAlbum);
            GetAlbumList();
        }

        private void TextEmpty()
        {
            if(tempAlbum != null)
            {
                AlbumList = tempAlbum;
            }
        }

        private async void Search(string para)
        {
            if (IsSearchVisible)
            {
                if (para == string.Empty)
                {
                    IsSearchVisible = false;
                    AlbumList = tempAlbum;
                    return;
                }
                //查找
                var searchedAlbumList =  AlbumList.Where(a => a.Title.Contains(para));
                if (searchedAlbumList != null)
                {
                    AlbumList = new ObservableCollection<Album>(searchedAlbumList);
                }
            }
            else
            {
                IsSearchVisible = true;
            }
        }

        private void SettingAlbum(Album album)
        {
            Debug.WriteLine(album);
        }

        private async void PlayAlbum(Album album)
        {
            var MusicList = new ObservableCollection<Music>(
                await musicService.GetMusicsByAlbumIdAsync(album.Id)
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

        private void SelectedAlbumChanged(Album album)
        {
            Debug.WriteLine(album);
        }

        /// <summary>
        /// 跳转到具体的专辑展示界面
        /// </summary>
        private void ConfirmAlbum(Album album)
        {
            if (isUpdating)
            {
                return;
            }
            NavigationParameters para = new NavigationParameters();
            para.Add("Album", album);
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                "PlayListView",
                Callback =>
                {
                    journal = Callback.Context.NavigationService.Journal;
                },
                para
            );
        }

        async void GetAlbumList()
        {
            AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(true));
            AlbumList = new ObservableCollection<Album>(await albumService.GetAllAsync());
            tempAlbum = AlbumList;
            AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(false));
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //重置选中项
            isUpdating = true;
            SelectedIndex = -1;
            isUpdating = false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) 
        {           
        }
    }
}
