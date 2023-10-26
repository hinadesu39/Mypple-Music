using Mypple_Music.Events;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class MusicListViewModel : NavigationViewModel
    {
        private IMusicService musicService;

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

        public DelegateCommand ChangeVisibilityCommand { set; get; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }

        public MusicListViewModel(IContainerProvider containerProvider, IMusicService musicService)
            : base(containerProvider)
        {
            this.musicService = musicService;
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            ChangeVisibilityCommand = new DelegateCommand(() =>
            {
                if (IsSearchVisible == true)
                    IsSearchVisible = false;
                else
                    IsSearchVisible = true;
            });
            Config();
        }

        private void SelectedMusicChanged(Music Music)
        {
            //设置播放状态
            var playingMusic = MusicList.FirstOrDefault(m => m.Status == Music.PlayStatus.StartPlay);
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
            SelectedMusic = Music;
            SelectedMusic.Status = Music.PlayStatus.StartPlay;

            //把当前播放列表发送给播放器待播放            
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(MusicList, MusicList.IndexOf(SelectedMusic), "MainView")
                );
        }

        async void Config()
        {
            IsSearchVisible = false;
            MusicList = new ObservableCollection<Music>(await musicService.GetAllAsync());
                      
        }
    }
}
