using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Prism.Commands;
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
    public class SettingsViewModel : NavigationViewModel
    {
        private readonly IRegionManager RegionManager;
        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set
            {
                menuBars = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel(IContainerProvider containerProvider, IRegionManager regionManager)
            : base(containerProvider)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            CreateMenuBar();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            RegionManager = regionManager;
        }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.NameSpace))
                return;
            RegionManager.Regions[PrismManager.SettingsiewRegionName].RequestNavigate(
                obj.NameSpace
            );
        }

        void CreateMenuBar()
        {
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "Palette",
                    Title = "个性化",
                    NameSpace = "SkinView"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "Information",
                    Title = "关于更多",
                    NameSpace = "AboutView"
                }
            );
        }
    }
}
