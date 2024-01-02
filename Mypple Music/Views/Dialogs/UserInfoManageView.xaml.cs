using Mypple_Music.Extensions;
using Prism.Events;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mypple_Music.Views.Dialogs
{
    /// <summary>
    /// InfoManageView.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoManageView : UserControl
    {
        public UserInfoManageView(IEventAggregator aggregator)
        {
            InitializeComponent();
            //注册消息提示
            aggregator.RegisterMessage(arg =>
            {
                InfoManageSnackBar.MessageQueue?.Enqueue(arg.Message);
            }, "InfoManage");
        }
    }
}
