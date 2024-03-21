using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Mypple_Music.Service;
using Mypple_Music.ViewModels;
using Mypple_Music.Views;
using Prism.DryIoc;
using Prism.Ioc;
using System.Configuration;
using System;
using System.Windows;
using System.Windows.Media;
using Mypple_Music.ViewModels.Dialogs;
using Mypple_Music.Views.Dialogs;
using Mypple_Music.Extensions;
using DryIoc;
using Mypple_Music.Events;
using Serilog;
using Mypple_Music.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Diagnostics;
using SixLabors.ImageSharp.Drawing;
using System.IO;

namespace Mypple_Music
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(
            ConfigurationUserLevel.None
        );

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected override void OnInitialized()
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            AppSession.IsDarkTheme = Convert.ToBoolean(
                ConfigurationManager.AppSettings.Get("IsDarkTheme")
            );
            theme.SetBaseTheme(AppSession.IsDarkTheme ? Theme.Dark : Theme.Light);

            var color = (Color)
                ColorConverter.ConvertFromString(ConfigurationManager.AppSettings.Get("Color"));

            theme.PrimaryLight = new ColorPair(color);
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color);

            paletteHelper.SetTheme(theme);

            ResourceDictionary resourceDictionary = new ResourceDictionary();
            if (AppSession.IsDarkTheme)
            {
                resourceDictionary.Source = new Uri(
                    "pack://application:,,,/Resource/DarkTheme.xaml"
                );
            }
            else
            {
                resourceDictionary.Source = new Uri(
                    "pack://application:,,,/Resource/LightTheme.xaml"
                );
            }
            Application.Current.Resources.MergedDictionaries[0] = resourceDictionary;

            var imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, "image");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = containerRegistry.GetContainer();

            var log = new LoggerConfiguration().WriteTo
                            .File("log/Mypple Music.log")
                            .CreateLogger();
            containerRegistry.RegisterInstance<ILogger>(log);
            log.Information("Log_Loaded");

            containerRegistry
                .GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry
                .GetContainer()
                .RegisterInstance(@"http://localhost", serviceKey: "webUrl");

            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.Register<ILyricService, LyricService>();
            containerRegistry.Register<IMusicService, MusicService>();
            containerRegistry.Register<IArtistService, ArtistService>();
            containerRegistry.Register<IAlbumService, AlbumService>();
            containerRegistry.Register<IPlayListService, PlayListService>();
            containerRegistry.Register<ILoginService, LoginService>();

            containerRegistry.RegisterForNavigation<NowToListenView, NowToListenViewModel>();
            containerRegistry.RegisterForNavigation<BroadcastView, BroadcastViewModel>();
            containerRegistry.RegisterForNavigation<BrowserView, BrowserViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<MusicListView, MusicListViewModel>();
            containerRegistry.RegisterForNavigation<LyricView, LyricViewModel>();
            containerRegistry.RegisterForNavigation<AlbumView, AlbumViewModel>();
            containerRegistry.RegisterForNavigation<RecentPostsView, RecentPostsViewModel>();
            containerRegistry.RegisterForNavigation<PlayListView, PlayListViewModel>();
            containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>();
            containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>();
            containerRegistry.RegisterForNavigation<AddPlayListView, AddPlayListViewModel>();
            containerRegistry.RegisterForNavigation<AllPlayListsView, AllPlayListsViewModel>();
            containerRegistry.RegisterForNavigation<ArtistView, ArtistViewModel>();
            containerRegistry.RegisterForNavigation<MusicWithArtistView, MusicWithArtistViewModel>();
            containerRegistry.RegisterForNavigation<AddMusicView, AddMusicViewModel>();
            containerRegistry.RegisterForNavigation<MusicWithAlbumView, MusicWithAlbumViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<UserCenterView, UserCenterViewModel>();
            containerRegistry.RegisterForNavigation<UserInfoManageView, UserInfoManageViewModel>();
            containerRegistry.RegisterForNavigation<SearchView, SearchViewModel>();
            containerRegistry.RegisterForNavigation<QuestionView, QuestionViewModel>();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.ContextMenu.PlacementTarget = btn;
            btn.ContextMenu.Placement = PlacementMode.Bottom;
            btn.ContextMenu.IsOpen = true;
        }
    }
}
