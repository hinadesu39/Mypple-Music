using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TagLib.IFD.Tags;

namespace Mypple_Music.ViewModels.Dialogs
{
    public class AddPlayListViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SetPicCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged();
            }
        }

        private string picUrl;

        public string PicUrl
        {
            get { return picUrl; }
            set
            {
                picUrl = value;
                RaisePropertyChanged();
            }
        }

        public AddPlayListViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            SetPicCommand = new DelegateCommand(SetPic);
        }

        private void SetPic()
        {
            // 创建一个 OpenFileDialog 的实例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置初始目录为 C:\
            openFileDialog.InitialDirectory = "C:\\Users\\Hinadesu\\Pictures";
            // 设置过滤器为图片文件
            openFileDialog.Filter = "Music files (*.jpg;*.png;)|*.jpg;*.png;|All files (*.*)|*.*";
            // 设置标题为 Select an image file
            openFileDialog.Title = "选择图片";
            // 显示对话框，并判断返回值
            openFileDialog.ShowDialog();
            // 获取用户选择的文件路径
            string filePath = openFileDialog.FileName;
            if (filePath == string.Empty)
                return;
            PicUrl = filePath;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(Title))
                return;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                var playList = new PlayList()
                {
                    Title = Title,
                    Description = Description,
                    LocalPicUrl = PicUrl
                };
                DialogParameters parameters = new DialogParameters();
                parameters.Add("Value", playList);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        public Task OnDialogOpendAsync(IDialogParameters parameters)
        {
           return Task.CompletedTask;
        }
    }
}
