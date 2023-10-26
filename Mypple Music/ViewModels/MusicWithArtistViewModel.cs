using Mypple_Music.Models;
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

        public MusicWithArtistViewModel(IContainerProvider containerProvider)
            : base(containerProvider)
        {
            Albums = new ObservableCollection<Album>();
            Config();
        }

        void Config()
        {
            
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Artist"))
            {
                this.artist = navigationContext.Parameters.GetValue<Artist>("Artist");
            }
        }
    }
}
