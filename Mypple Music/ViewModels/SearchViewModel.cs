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
using System.Windows.Input.StylusPlugIns;

namespace Mypple_Music.ViewModels
{
    public class SearchViewModel : NavigationViewModel
    {
        #region Field

        private readonly IRegionManager regionManager;
        private readonly IMusicService musicService;
        private string whichKeyWords;

        #endregion

        #region Property

        public DelegateCommand<Music> PauseOrPlayCommand { set; get; }
        public DelegateCommand<Music> ToPlayMusicCommand { set; get; }
        public DelegateCommand<Artist> ConfirmArtistCommand { set; get; }
        public DelegateCommand<Album> ConfirmAlbumCommand { set; get; }
        public DelegateCommand<Album> PlayAlbumCommand { set; get; }
        public DelegateCommand<string> ExecuteCommand { set; get; }
        private int selectedArtist = -1;

        public int SelectedArtist
        {
            get { return selectedArtist; }
            set { selectedArtist = value; RaisePropertyChanged(); }
        }

        private int selectedAlbum = -1;

        public int SelectedAlbum
        {
            get { return selectedAlbum; }
            set { selectedAlbum = value; RaisePropertyChanged(); }
        }


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

        private ObservableCollection<Music> musics;

        public ObservableCollection<Music> Musics
        {
            get { return musics; }
            set { musics = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Album> albums;

        public ObservableCollection<Album> Albums
        {
            get { return albums; }
            set { albums = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<Artist> artists;

        public ObservableCollection<Artist> Artists
        {
            get { return artists; }
            set { artists = value; RaisePropertyChanged(); }
        }

        private string keyWords;

        public string KeyWords
        {
            get { return keyWords; }
            set { keyWords = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Ctor

        public SearchViewModel(IContainerProvider containerProvider, IMusicService musicService, IRegionManager regionManager) : base(containerProvider)
        {
            this.regionManager = regionManager;
            this.musicService = musicService;
            PauseOrPlayCommand = new DelegateCommand<Music>(PauseOrPlay);
            ToPlayMusicCommand = new DelegateCommand<Music>(ToPlayMusic);
            ConfirmAlbumCommand = new DelegateCommand<Album>(ConfirmAlbum);
            ConfirmArtistCommand = new DelegateCommand<Artist>(ConfirmArtist);
            PlayAlbumCommand = new DelegateCommand<Album>(PlayAlbum);
            ExecuteCommand = new DelegateCommand<string>(Execute);
        }


        #endregion

        #region Command

        private void Execute(string obj)
        {
            NavigationParameters para = new NavigationParameters();
            if(obj == "MusicListView")
            {
                para.Add("SearchedResult", musics);
            }
            else if(obj == "AlbumView")
            {
                para.Add("SearchedResult", albums);
            }
            else
            {
                para.Add("SearchedResult", artists);
            }
            
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj, para);
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

        private void ConfirmAlbum(Album album)
        {
            if (album == null)
                return;
            NavigationParameters para = new NavigationParameters();
            para.Add("Album", album);
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("MusicWithAlbumView", para);
            SelectedAlbum = -1;
        }

        private void ConfirmArtist(Artist artist)
        {
            if (artist == null)
                return;
            NavigationParameters para = new NavigationParameters();
            para.Add("Artist", artist);
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("MusicWithArtistView", para);
            SelectedArtist = -1;
        }

        private void ToPlayMusic(Music music)
        {
            if(music == null)
                return;
            //设置播放状态
            var playingMusic = Musics.FirstOrDefault(
                m => m.Status == Music.PlayStatus.StartPlay
            );
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
            music.Status = Music.PlayStatus.StartPlay;

            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(
                        Musics,
                        Musics.IndexOf(music),
                        "MainView"
                    )
                );
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(music, "LyricView"));
        }

        private void PauseOrPlay(Music music)
        {
            //if (SelectedMusic != music)
            //{
            //    ToPlayMusic(music);
            //    return;
            //}
            if (music.Status == Music.PlayStatus.PausePlay)
            {
                music.Status = Music.PlayStatus.StartPlay;
            }
            else if (music.Status == Music.PlayStatus.StartPlay)
            {
                music.Status = Music.PlayStatus.PausePlay;
            }
            else
            {
                ToPlayMusic(music);
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.ContainsKey("KeyWords"))
            {
                KeyWords = navigationContext.Parameters.GetValue<string>("KeyWords");
                //return if the page was loaded
                if (whichKeyWords != null && whichKeyWords == KeyWords)
                {
                    return;
                }
                whichKeyWords = KeyWords;
            }
            else
            {
                return;
            }
            UpdateLoading(true);
            var res = await musicService.GetByKeywordsAsync(KeyWords);
            if (res != null)
            {
                Musics = new ObservableCollection<Music>(res.musics);
                Artists = new ObservableCollection<Artist>(res.artists);
                Albums = new ObservableCollection<Album>(res.albums);
            }
            UpdateLoading(false);

        }
        #endregion
    }
}
