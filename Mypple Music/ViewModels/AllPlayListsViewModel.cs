using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class AllPlayListsViewModel : NavigationViewModel
    {
        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set { isSearchVisible = value; RaisePropertyChanged(); }
        }
        public DelegateCommand ChangeVisibilityCommand { get; set; }

        public AllPlayListsViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            ChangeVisibilityCommand = new DelegateCommand(ChangeVisibility);
        }

        private void ChangeVisibility()
        {
            throw new NotImplementedException();
        }
    }
}
