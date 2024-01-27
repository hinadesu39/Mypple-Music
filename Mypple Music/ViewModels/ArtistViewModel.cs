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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.ViewModels
{
    public class ArtistViewModel : NavigationViewModel
    {
        #region Field
        private readonly IArtistService artistService;
        private readonly IRegionManager RegionManager;
        private bool isSearchedResult;
        private ObservableCollection<Artist> tempArtist;
        #endregion

        #region Property
        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set { isSearchVisible = value; RaisePropertyChanged(); }
        }

        private int selectedArtistIndex = 0;

        public int SelectedArtistIndex
        {
            get { return selectedArtistIndex; }
            set { selectedArtistIndex = value; RaisePropertyChanged(); }
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

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Artist> NavigateCommand { get; set; }
        #endregion

        #region Ctor
        public ArtistViewModel(IContainerProvider containerProvider, IRegionManager regionManager, IArtistService artistService)
            : base(containerProvider)
        {
           
            this.artistService = artistService;
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            NavigateCommand = new DelegateCommand<Artist>(Navigate);
            RegionManager = regionManager;
            //Init();
        }
        #endregion

        #region Command
        private void TextEmpty()
        {
            if (tempArtist != null)
            {
                Artists = tempArtist;
                SelectedArtistIndex = 0;
            }
        }

        private void Search(string para)
        {
            if (IsSearchVisible)
            {
                if (para == string.Empty)
                {
                    IsSearchVisible = false;
                    Artists = tempArtist;
                    return;
                }
                //查找
                var searchedArtistList = Artists.Where(a => a.Name.Contains(para, StringComparison.OrdinalIgnoreCase));
                if (searchedArtistList != null)
                {
                    Artists = new ObservableCollection<Artist>(searchedArtistList);
                    SelectedArtistIndex = 0;
                }
            }
            else
            {
                IsSearchVisible = true;
            }
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

        async void Init()
        {
            UpdateLoading(true);
            var artist = await artistService.GetAllAsync();
            if (artist != null)
            {
                Artists = new ObservableCollection<Artist>(artist);
            }
            else
                eventAggregator.SendMessage("连接出现问题~~~");
            tempArtist = Artists;
            UpdateLoading(false);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

            if (isSearchedResult)
            {
                isSearchedResult = false;
                Artists = null;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("SearchedResult"))
            {
                Artists = navigationContext.Parameters.GetValue<ObservableCollection<Artist>>("SearchedResult");
                tempArtist = Artists;
                isSearchedResult = true;
                Navigate(Artists[0]);
            }
            else if (Artists == null)
            {
                Init();
                SelectedArtistIndex = 0;
            }
        }
        #endregion
    }
}
