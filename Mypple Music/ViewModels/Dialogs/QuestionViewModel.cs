using MaterialDesignThemes.Wpf;
using Mypple_Music.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels.Dialogs
{
    public class QuestionViewModel : BindableBase, IDialogHostAware
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }


        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public QuestionViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cance);
        }

        private void Cance()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters parameters = new DialogParameters();
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        public Task OnDialogOpendAsync(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Title") && parameters.ContainsKey("Content"))
            {
                Title = parameters.GetValue<string>("Title");
                Content = parameters.GetValue<string>("Content");
            }
            return Task.CompletedTask;
        }
    }
}
