using MaterialDesignThemes.Wpf;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Views.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mypple_Music.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogHostService dialogHostService;
        public MainView(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogHostService dialogHostService)
        {
            InitializeComponent();
            this.regionManager = regionManager;
            this.dialogHostService = dialogHostService;
            AppSession.EventAggregator = eventAggregator;
            Loaded += MainView_Loaded;

            //注册消息通知
            eventAggregator.RegisterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg.Message);
            });

            //加载动画注册
            eventAggregator
                .GetEvent<LoadingEvent>()
                .Subscribe(
                    arg =>
                    {
                        DialogHost.IsOpen = arg.IsLoading;
                        if (arg.IsLoading == true)
                        {
                            DialogHost.DialogContent = new ProgressView();
                        }
                    },
                    m =>
                    {
                        return m.filter == "MainView";
                    }
                );
            ColorZone.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            //ColorZone.MouseDoubleClick += (s, e) =>
            //{
            //    if (this.WindowState == WindowState.Normal)
            //    {

            //        this.WindowState = WindowState.Maximized;
            //    }
            //    else
            //    {
            //        this.WindowState = WindowState.Normal;
            //    }
            //};

            btn_Min.Click += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };
            btn_Max.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            };
            btn_Close.Click += async (s, e) =>
            {
                //var dialogRes = await dialogHostService.Question("温馨提示", $"确认退出系统?");
                //if (dialogRes.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
        }

        WindowAccentCompositor windowAccentCompositor = null;

        public void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            windowAccentCompositor = new(
                this,
                false,
                c =>
                {
                    //没有可用的模糊特效
                    c.A = 255;
                    Background = new SolidColorBrush(c);
                }
            );
            windowAccentCompositor.Color = AppSession.IsDarkTheme
                ? Color.FromArgb(180, 0, 0, 0)
                : Color.FromArgb(180, 255, 255, 255);
            windowAccentCompositor.IsEnabled = true;

            AppSession.EventAggregator
                .GetEvent<ThemeChangedEvent>()
                .Subscribe(
                    arg =>
                    {
                        windowAccentCompositor.Color = AppSession.IsDarkTheme
                            ? Color.FromArgb(180, 0, 0, 0)
                            : Color.FromArgb(180, 255, 255, 255);
                        windowAccentCompositor.DarkMode = AppSession.IsDarkTheme;
                        windowAccentCompositor.IsEnabled = true;
                    },
                    f =>
                    {
                        return f.filter == "MainView";
                    }
                );
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                "RecentPostsView"
            );
        }
    }
}
