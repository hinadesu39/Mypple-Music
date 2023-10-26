using Mypple_Music.Models;
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
    public class RecentPostsViewModel : NavigationViewModel
    {
        private ObservableCollection<Album> albumList;

        public ObservableCollection<Album> AlbumList
        {
            get { return albumList; }
            set { albumList = value; RaisePropertyChanged(); }
        }


        public DelegateCommand ConfirmAlbumCommand { get; set; }


        public RecentPostsViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            ConfirmAlbumCommand = new DelegateCommand(ConfirmAlbum);
        }
     




        private void ConfirmAlbum()
        {
            throw new NotImplementedException();
        }
    }
}
