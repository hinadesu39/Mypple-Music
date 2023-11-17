
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class MusicWithArtistViewModel : NavigationViewModel
    {
        private bool isUpdating;
        private IMusicService musicService;
        private IAlbumService albumService;

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

        private int countOfAlbum;

        public int CountOfAlbum
        {
            get { return countOfAlbum; }
            set
            {
                countOfAlbum = value;
                RaisePropertyChanged();
            }
        }

        private int countOfMusic;

        public int CountOfMusic
        {
            get { return countOfMusic; }
            set
            {
                countOfMusic = value;
                RaisePropertyChanged();
            }
        }

        private Music selectedMusic;

        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set { selectedMusic = value; }
        }

        private Artist artist;

        public Artist Artist
        {
            get { return artist; }
            set
            {
                artist = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Album> albums;

        public ObservableCollection<Album> Albums
        {
            get { return albums; }
            set
            {
                albums = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }
        public DelegateCommand<Music> FocuseChangedCommand { set; get; }
        public DelegateCommand ChangeVisibilityCommand { set; get; }

        public MusicWithArtistViewModel(
            IContainerProvider containerProvider,
            IAlbumService albumService,
            IMusicService musicService
        )
            : base(containerProvider)
        {
            Albums = new ObservableCollection<Album>();
            this.albumService = albumService;
            this.musicService = musicService;

            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            FocuseChangedCommand = new DelegateCommand<Music>(FocuseChanged);
            ChangeVisibilityCommand = new DelegateCommand(() =>
            {
                if (IsSearchVisible == true)
                    IsSearchVisible = false;
                else
                    IsSearchVisible = true;
            });
        }

        private void FocuseChanged(Music music)
        {
            if (isUpdating)
                return;
            var album = Albums.FirstOrDefault(a => a.MusicIndex != -1 && a.Id != music.AlbumId);
            if (album == null)
                return;
            isUpdating = true;
            album.MusicIndex = -1;
            isUpdating = false;         
        }

        private void SelectedMusicChanged(Music music)
        {
            var album = Albums.FirstOrDefault(a => a.Id == music.AlbumId);           
            //设置播放状态
            var playingMusic = Albums
                .SelectMany(a => a.MusicList)
                .Where(a => a.Status == Music.PlayStatus.StartPlay);
            if (playingMusic != null)
                playingMusic.ToList().ForEach(a => a.Status = Music.PlayStatus.StopPlay);

            SelectedMusic = music;
            SelectedMusic.Status = Music.PlayStatus.StartPlay;
            album.MusicIndex = album.MusicList.IndexOf(SelectedMusic);
            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(
                        album.MusicList,
                        album.MusicList.IndexOf(SelectedMusic),
                        "MainView"
                    )
                );
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(music, "LyricView"));
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Artist"))
            {
                this.Artist = navigationContext.Parameters.GetValue<Artist>("Artist");
                AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(true));
                Albums = new ObservableCollection<Album>(
                    await albumService.GetAlbumsByArtistIdAsync(Artist.Id)
                );
                CountOfAlbum = Albums.Count;
                foreach (var album in Albums)
                {
                    album.MusicList = new ObservableCollection<Music>(
                        await musicService.GetMusicsByAlbumIdAsync(album.Id)
                    );
                    CountOfMusic += album.MusicList.Count;
                }
                AppSession.EventAggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(false));
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            CountOfAlbum = 0;
            CountOfMusic = 0;
        }
    }
}
