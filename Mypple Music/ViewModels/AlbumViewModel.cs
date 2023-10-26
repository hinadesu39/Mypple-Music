using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
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
        private ObservableCollection<Album> albumList;

        public ObservableCollection<Album> AlbumList
        {
            get { return albumList; }
            set { albumList = value; RaisePropertyChanged(); }
        }


        public DelegateCommand ConfirmAlbumCommand { get; set; }


        public AlbumViewModel(IContainerProvider containerProvider, IAlbumService albumService) : base(containerProvider)
        {
            this.albumService = albumService;
            ConfirmAlbumCommand = new DelegateCommand(ConfirmAlbum);
            GetAlbumList();
        }

        private void ConfirmAlbum()
        {
            
        }

       async void GetAlbumList()
        {
            AlbumList = new ObservableCollection<Album>(await albumService.GetAllAsync());
            Debug.WriteLine(AlbumList);
        }
    }
}
