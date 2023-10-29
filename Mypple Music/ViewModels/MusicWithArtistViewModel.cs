using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class MusicWithArtistViewModel : NavigationViewModel
    {
        private IMusicService musicService;
        private IAlbumService albumService;
        private Artist artist;

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

        public MusicWithArtistViewModel(IContainerProvider containerProvider, IAlbumService albumService, IMusicService musicService)
            : base(containerProvider)
        {
            Albums = new ObservableCollection<Album>();
            this.albumService = albumService;
            this.musicService = musicService;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Artist"))
            {
                this.artist = navigationContext.Parameters.GetValue<Artist>("Artist");
                await albumService.get
            }
        }
    }
}
