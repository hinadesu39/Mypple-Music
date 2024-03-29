﻿
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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using Mypple_Music.Extensions;
using Mypple_Music.Models.Request;

namespace Mypple_Music.ViewModels
{
    public class MusicWithArtistViewModel : NavigationViewModel
    {
        #region Field
        private bool isUpdating;
        private Artist whichArtist;
        private readonly IMusicService musicService;
        private readonly IAlbumService albumService;
        private readonly IPlayListService playListService;
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

        public DelegateCommand<Music> ToPlayMusicCommand { set; get; }
        public DelegateCommand<Music> FocuseChangedCommand { set; get; }
        public DelegateCommand ChangeVisibilityCommand { set; get; }
        public DelegateCommand<PlayList> AddToPlayListCommand { set; get; }
        #endregion

        #region Ctor
        public MusicWithArtistViewModel(
            IContainerProvider containerProvider,
            IAlbumService albumService,
            IMusicService musicService,
            IPlayListService playListService
        )
            : base(containerProvider)
        {
            Albums = new ObservableCollection<Album>();
            this.albumService = albumService;
            this.musicService = musicService;
            this.playListService = playListService;

            ToPlayMusicCommand = new DelegateCommand<Music>(ToPlayMusic);
            FocuseChangedCommand = new DelegateCommand<Music>(FocuseChanged);
            ChangeVisibilityCommand = new DelegateCommand(() =>
            {
                if (IsSearchVisible == true)
                    IsSearchVisible = false;
                else
                    IsSearchVisible = true;
            });
            AddToPlayListCommand = new DelegateCommand<PlayList>(AddToPlayList);

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

        /// <summary>
        /// 保证整个列表选中歌曲的唯一性
        /// </summary>
        /// <param name="music"></param>
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

        private void ToPlayMusic(Music music)
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
                if (whichArtist == Artist)
                    return;
                whichArtist = Artist;
                UpdateLoading(true);
                Albums = new ObservableCollection<Album>(
                    await albumService.GetAlbumsByArtistIdAsync(Artist.Id)
                );
                CountOfAlbum = Albums.Count;
                var count = 0;
                foreach (var album in Albums)
                {
                    album.MusicList = new ObservableCollection<Music>(
                        await musicService.GetMusicsByAlbumIdAsync(album.Id)
                    );
                    count += album.MusicList.Count;
                }
                CountOfMusic = count;
                UpdateLoading(false);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            CountOfAlbum = 0;
            CountOfMusic = 0;
        }
        #endregion
    }
}
