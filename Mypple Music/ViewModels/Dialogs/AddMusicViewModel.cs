using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
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
        private ObservableCollection<Music> tempMusic;
        private Guid playListId;

        public string DialogHostName { get; set; }
        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
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
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            Config();
        }

        private void TextEmpty()
        {
            if (tempMusic != null)
            {
                MusicList = tempMusic;
            }
        }

        private void Search(string obj)
        {
            var searchedMusicList = MusicList.Where(m => m.Title.Contains(obj));
            MusicList = new ObservableCollection<Music>(searchedMusicList);
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
            var toAddMusic = MusicList.Where(m => m.IsSelected).ToArray();
            if (toAddMusic.Count() == 0)
                return;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                var musicAddToPlayListRequest = new MusicAddToPlayListRequest(playListId, toAddMusic);
                DialogParameters parameters = new DialogParameters();
                parameters.Add("Value", musicAddToPlayListRequest);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        public async void Config()
        {
            MusicList = new ObservableCollection<Music>(await musicService.GetAllAsync());
            tempMusic = MusicList;
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
            if (parameters.ContainsKey("Id"))
            {
                playListId = parameters.GetValue<Guid>("Id");
            }
        }
    }
}
