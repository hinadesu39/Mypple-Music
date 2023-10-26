using Mypple_Music.Extensions;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public MainView()
        {
            InitializeComponent();
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
            btn_Close.Click += (s, e) =>
            {
                //var dialogRes = await dialogHostService.Question("温馨提示", $"确认退出系统?");
                //if (dialogRes.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
        }
    }
}
