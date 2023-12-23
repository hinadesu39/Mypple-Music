using Microsoft.Win32;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class UserCenterViewModel : NavigationViewModel
    {
        private ILoginService loginService;
        private IRegionNavigationJournal journal;
        private readonly IDialogHostService dialog;

        public DelegateCommand ChangeAvatarCommand { set; get; }
        public DelegateCommand GoBackCommand { set; get; }
        public DelegateCommand SaveCommand { set; get; }
        public DelegateCommand<string> ExecuteCommand { set; get; }


        private SimpleUser userDto;

        public SimpleUser UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }

        public UserCenterViewModel(IContainerProvider containerProvider, ILoginService loginService, IDialogHostService dialog) : base(containerProvider)
        {
            ChangeAvatarCommand = new DelegateCommand(ChangeAvatar);
            GoBackCommand = new DelegateCommand(GoBack);
            SaveCommand = new DelegateCommand(Save);
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.loginService = loginService;
            this.dialog = dialog;
        }

        private async void Execute(string obj)
        {
            var selectedIndex = 0;
            if(obj == "BingdingEmail")
            {
                selectedIndex = 0;
            }
            else if(obj == "BingdingPhone")
            {
                selectedIndex = 1;
            }
            else if(obj == "ChangeEmail")
            {
                selectedIndex = 2;
            }
            else if(obj == "ChangePhone")
            {
                selectedIndex = 3;
            }
            else if (obj == "ChangePassword")
            {
                selectedIndex = 4;
            }

            DialogParameters para = new DialogParameters();
            para.Add("SelectedIndex", selectedIndex);
            para.Add("User", UserDto);
            var dialogRes = await dialog.ShowDialog("InfoManageView", para);
        }


        private async void Save()
        {
            var res = await loginService.UpdateUserInfo(new UpdateUserInfoRequest(UserDto.UserName, UserDto.Gender, UserDto.BirthDay, UserDto.UserAvatar));
            if (res != null)
            {
                if (res.Status)
                {
                    eventAggregator.SendMessage(res.Message);
                }
                else
                {
                    eventAggregator.SendMessage(res.Message);
                }
            }
        }

        private void GoBack()
        {
            if (journal != null && journal.CanGoBack)
                journal.GoBack();
        }

        private async void ChangeAvatar()
        {
            // 创建一个 OpenFileDialog 的实例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置初始目录为 C:\ 
            openFileDialog.InitialDirectory = "C:\\";
            // 设置过滤器为图片文件
            openFileDialog.Filter = "Image files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|All files (*.*)|*.*";
            // 设置标题为 Select an image file
            openFileDialog.Title = "Select an image file";
            // 显示对话框，并判断返回值
            openFileDialog.ShowDialog();
            // 获取用户选择的文件路径
            string filePath = openFileDialog.FileName;
            if (filePath == string.Empty) return;

            var newPath = await loginService.UploadAsync(filePath);
            if (newPath == null) return;

            UserDto.UserAvatar = new Uri(newPath);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.ContainsKey("User"))
            {
                UserDto = navigationContext.Parameters.GetValue<SimpleUser>("User");
            }
            if (navigationContext.Parameters.ContainsKey("journal"))
            {
                journal = navigationContext.Parameters.GetValue<IRegionNavigationJournal>("journal");
            }
        }
    }
}
