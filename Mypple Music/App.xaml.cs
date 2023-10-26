using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Mypple_Music.Service;
using Mypple_Music.ViewModels;
using Mypple_Music.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Configuration;
using System;
using System.Windows;
using System.Windows.Media;
using Mypple_Music.ViewModels.Dialogs;
using Mypple_Music.Views.Dialogs;
using Mypple_Music.Extensions;
using DryIoc;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;

namespace Mypple_Music
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {      
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                  
        
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            var IsDarkTheme = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsDarkTheme"));
            theme.SetBaseTheme(IsDarkTheme ? Theme.Dark : Theme.Light);


            var color = (Color)ColorConverter.ConvertFromString(ConfigurationManager.AppSettings.Get("Color"));

            theme.PrimaryLight = new ColorPair(color);
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color);

            paletteHelper.SetTheme(theme);
            base.OnInitialized();
            //var dialog = Container.Resolve<IDialogService>();
            //dialog.ShowDialog("LoginView", callBack =>
            //{
            //    if (callBack.Result == ButtonResult.OK)
            //    {
            //        var service = App.Current.MainWindow.DataContext as IConfigureService;
            //        if (service != null)
            //        {
            //            service.Configure();
            //        }
            //        base.OnInitialized();
            //    }
            //    else
            //    {
            //        Environment.Exit(0);
            //        return;
            //    }

            //});

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //var factory = new NLogLoggerFactory();
            //_logger = factory.CreateLogger("NLog.config");
            //containerRegistry.RegisterInstance<ILogger>(_logger);
            var container = containerRegistry.GetContainer();
            container.RegisterMany(new[] { typeof(IMediator).GetAssembly() }, Registrator.Interfaces);

            containerRegistry.GetContainer().Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost", serviceKey: "webUrl");

            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.Register<ILyricService, LyricService>();
            containerRegistry.Register<IMusicService, MusicService>();
            containerRegistry.Register<IArtistService, ArtistService>();
            containerRegistry.Register<IAlbumService, AlbumService>();
            
          
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

        }
        //注册外部服务方法如下
        //protected override IContainerExtension CreateContainerExtension()
        //{
        //    var serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(configure =>
        //    {
        //        configure.ClearProviders();
        //        configure.SetMinimumLevel(LogLevel.Trace);
        //        configure.AddNLog();
        //    });

        //    return new DryIocContainerExtension(new Container(CreateContainerRules())
        //        .WithDependencyInjectionAdapter(serviceCollection));
        //}
    }
}
