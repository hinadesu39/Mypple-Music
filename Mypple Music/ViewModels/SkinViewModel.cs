using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using Mypple_Music.Events;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Mypple_Music.ViewModels
{
    public class SkinViewModel : NavigationViewModel
    {
        //打开当前应用程序的配置文件
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(
            ConfigurationUserLevel.None
        );
        public DelegateCommand<object> ChangeHueCommand { set; get; }
        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? Theme.Dark : Theme.Light));

                    AppSession.IsDarkTheme = value;

                    //修改配置内容
                    config.AppSettings.Settings["IsDarkTheme"].Value = value.ToString();

                    //保存更改
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");

                    //切换自定义主题
                    ResourceDictionary resourceDictionary = new ResourceDictionary();
                    if (value)
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
                    //通知主题改变
                    AppSession.EventAggregator
                        .GetEvent<ThemeChangedEvent>()
                        .Publish(new ThemeChangedModel(value));
                }
            }
        }

        public SkinViewModel(IContainerProvider containerProvider)
            : base(containerProvider)
        {
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        private void ChangeHue(object? obj)
        {
            //主题色号
            var hue = (Color)obj!;
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(hue.Lighten());
            theme.PrimaryMid = new ColorPair(hue);
            theme.PrimaryDark = new ColorPair(hue.Darken());

            paletteHelper.SetTheme(theme);

            config.AppSettings.Settings["Color"].Value = obj.ToString();

            //保存更改
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsDarkTheme = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsDarkTheme"));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
