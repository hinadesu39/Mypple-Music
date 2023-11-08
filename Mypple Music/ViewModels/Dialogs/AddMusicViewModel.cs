using MaterialDesignThemes.Wpf;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels.Dialogs
{
    public class AddMusicViewModel : BindableBase, IDialogHostAware
    {
        private IMusicService musicService;

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }

        public bool? IsAllItemsSelected
        {
            get
            {
                if(MusicList == null)
                    return false;
                var selected = MusicList.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, MusicList);
                    RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<Music> musicList;

        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set
            {
                musicList = value;
                RaisePropertyChanged();
            }
        }

        public AddMusicViewModel(IMusicService musicService)
        {
            this.musicService = musicService;
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            Config();

        }

        private static void SelectAll(bool select, IEnumerable<Music> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        private void SelectedMusicChanged(Music music)
        {
            music.IsSelected = true;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        public async void Config()
        {
            MusicList = new ObservableCollection<Music>(await musicService.GetAllAsync());
            foreach (var model in MusicList)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(Music.IsSelected))
                        RaisePropertyChanged("IsAllItemsSelected");
                };
            }
        }

        public async void OnDialogOpend(IDialogParameters parameters)
        {
            return;
        }
    }
}
