using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.ViewModels
{
    public class ArtistViewModel : NavigationViewModel
    {
        private IArtistService artistService;
        private readonly IRegionManager RegionManager;

        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set { isSearchVisible = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Artist> artists;

        public ObservableCollection<Artist> Artists
        {
            get { return artists; }
            set
            {
                artists = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ChangeVisibilityCommand { get; set; }
        public DelegateCommand<Artist> NavigateCommand { get; set; }

        public ArtistViewModel(IContainerProvider containerProvider, IRegionManager regionManager, IArtistService artistService)
            : base(containerProvider)
        {
           
            this.artistService = artistService;
            ChangeVisibilityCommand = new DelegateCommand(ChangeVisibility);
            NavigateCommand = new DelegateCommand<Artist>(Navigate);
            RegionManager = regionManager;
            Config();
        }

        private void ChangeVisibility()
        {
            
        }

        private void Navigate(Artist artist)
        {
            if (artist != null)
            {
                NavigationParameters para = new NavigationParameters { { "Artist", artist } };
                RegionManager.Regions[PrismManager.ArtistRegionName].RequestNavigate(
                    "MusicWithArtistView",
                    para
                );
            }
        }

        async void Config()
        {            
            Artists = new ObservableCollection<Artist>(await artistService.GetAllAsync());
        }
    }
}
