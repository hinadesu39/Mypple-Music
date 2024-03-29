﻿using Mypple_Music.Common;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Mypple_Music.ViewModels
{
    public class PlayListViewModel : NavigationViewModel
    {
        #region Field
        private bool isUpdating;
        private Guid whichPlayList;
        private readonly IMusicService musicService;
        private readonly IDialogHostService dialog;
        private readonly IPlayListService playListService;
        private readonly IDialogHostService dialogHostService;
        private ObservableCollection<Music> tempMusic;
        private Guid playListId;
        private Music musicToEdit;
        private IRegionNavigationJournal journal;
        #endregion

        #region Property
        private ObservableCollection<PlayList> allPlayLists = AppSession.AllPlayLists;

        public ObservableCollection<PlayList> AllPlayLists
        {
            get { return allPlayLists; }
            set
            {
                allPlayLists = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 下载是否进行
        /// </summary>
        private bool isDownloading;

        public bool IsDownloading
        {
            get { return isDownloading; }
            set
            {
                isDownloading = value;
                RaisePropertyChanged();
            }
        }

        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set
            {
                isSearchVisible = value;
                RaisePropertyChanged();
            }
        }

        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                RaisePropertyChanged();
            }
        }

        private double duration;

        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                RaisePropertyChanged();
            }
        }

        private PlayList playList;

        public PlayList PlayList
        {
            get { return playList; }
            set
            {
                playList = value;
                RaisePropertyChanged();
            }
        }

        private Music selectedMusic;

        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set
            {
                selectedMusic = value;
                RaisePropertyChanged();
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

        public DelegateCommand<string> SearchCommand { get; set; }
        public DelegateCommand TextEmptyCommand { get; set; }
        public DelegateCommand<Music> ToPlayMusicCommand { set; get; }
        public DelegateCommand<Music> PauseOrPlayCommand { set; get; }
        public DelegateCommand<string> ExecuteCommand { get; set; }
        public DelegateCommand DownloadAllCommand { set; get; }
        public DelegateCommand<PlayList> AddToPlayListCommand { set; get; }
        public DelegateCommand RemoveMusicFromPlayListCommand { set; get; }

        #endregion

        #region Ctor
        public PlayListViewModel(
            IContainerProvider containerProvider,
            IMusicService musicService,
            IDialogHostService dialog,
            IPlayListService playListService,
            IDialogHostService dialogHostService
        )
            : base(containerProvider)
        {
            this.dialog = dialog;
            this.musicService = musicService;
            this.playListService = playListService;
            this.dialogHostService = dialogHostService;
            SearchCommand = new DelegateCommand<string>(Search);
            TextEmptyCommand = new DelegateCommand(TextEmpty);
            ToPlayMusicCommand = new DelegateCommand<Music>(ToPlayMusic);
            PauseOrPlayCommand = new DelegateCommand<Music>(PauseOrPlay);
            ExecuteCommand = new DelegateCommand<string>(Execute);
            DownloadAllCommand = new DelegateCommand(DownloadAllAsync);
            AddToPlayListCommand = new DelegateCommand<PlayList>(AddToPlayList);
            RemoveMusicFromPlayListCommand = new DelegateCommand(RemoveMusicFromPlayList);
        }


        #endregion

        #region Command

        private async void RemoveMusicFromPlayList()
        {
            //移除最后一首歌的时候要求整个播放列表要被删除
            if (MusicList.Count == 1)
            {
                DeletePlayList();
                return;
            }

            var res = await playListService.RemoveMusicFromPlayList(new RemoveMusicFromPlayListRequest(whichPlayList, SelectedMusic.Id));
            if (!res)
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}移除失败");
            }
            else
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}移除成功");
                MusicList.Remove(SelectedMusic);
            }

        }

        /// <summary>
        /// 添加单曲到播放列表
        /// </summary>
        /// <param name="list"></param>
        private async void AddToPlayList(PlayList list)
        {
            var musicAddToPlayListRequest = new MusicAddToPlayListRequest(list.Id, SelectedMusic);
            var res = await playListService.AddMusicToPlayListAsync(musicAddToPlayListRequest);
            if (res == null)
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}添加失败");
            }
            else
            {
                eventAggregator.SendMessage($"{SelectedMusic.Title}添加成功");
            }
        }

        private async void DownloadAllAsync()
        {
            IsDownloading = true;
            foreach (var m in MusicList)
            {
                try
                {
                    var res = await DownloadHelper.GetMusicAsync(m.AudioUrl);
                    if (res == "Download Successful")
                    {
                        eventAggregator.SendMessage($"{m.Title} 下载成功！");
                    }
                    else if (res == "File Exist")
                    {
                        eventAggregator.SendMessage($"{m.Title} 已存在！");
                    }
                    else
                    {
                        eventAggregator.SendMessage($"{m.Title} 下载失败,请稍后再试！");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    eventAggregator.SendMessage($"{m.Title} 下载失败,请稍后再试！");
                }
            }
            IsDownloading = false;
        }

        //主弹出框命令
        private async void Execute(string obj)
        {
            if (isUpdating)
                return;
            DialogParameters parameter = new DialogParameters();
            switch (obj)
            {
                case "添加歌曲":
                    parameter.Add("Id", playListId);
                    var dialogRes = await dialog.ShowDialog("AddMusicView", parameter);
                    if (dialogRes.Result == ButtonResult.OK)
                    {
                        var musicAddToPlayListRequest =
                            dialogRes.Parameters.GetValue<MusicAddToPlayListRequest>("Value");
                        var res = await playListService.AddMusicToPlayListAsync(
                            musicAddToPlayListRequest
                        );
                        if (res == null)
                        {
                            eventAggregator.SendMessage("添加失败");
                            return;
                        }
                        if (MusicList == null)
                        {
                            MusicList = new ObservableCollection<Music>(res);
                        }
                        MusicList.AddRange(res);
                        tempMusic = MusicList;
                    }
                    break;
                case "添加至播放列表":

                    break;
                case "删除播放列表":
                    DeletePlayList();
                    break;
            }
        }

        private async void DeletePlayList()
        {
            var dialogRes = await dialogHostService.Question("温馨提示", $"将会永久删除整个播放列表(真的很久！！)");
            if (dialogRes.Result != ButtonResult.OK) return;
            var res = await playListService.DeleteAsync(whichPlayList);
            if (!res)
            {
                eventAggregator.SendMessage($"{PlayList.Title},删除失败");
            }
            else
            {
                if (journal.CanGoBack)
                {
                    journal.GoBack();
                }
                eventAggregator.SendMessage($"{PlayList.Title},删除成功");
                eventAggregator.GetEvent<PlayListDeletedEvent>().Publish(new PlayListDeletedModel(PlayList));
                AllPlayLists.Remove(PlayList);
                AppSession.AllPlayLists.Remove(PlayList);
            }
        }

        private void PauseOrPlay(Music music)
        {
            if (SelectedMusic != music)
            {
                ToPlayMusic(music);
                return;
            }
            if (music.Status == Music.PlayStatus.PausePlay)
            {
                music.Status = Music.PlayStatus.StartPlay;
            }
            else if (music.Status == Music.PlayStatus.StartPlay)
            {
                music.Status = Music.PlayStatus.PausePlay;
            }
            else
            {
                ToPlayMusic(music);
            }
        }

        private void TextEmpty()
        {
            if (tempMusic != null)
            {
                MusicList = tempMusic;
            }
        }

        private void Search(string para)
        {
            if (IsSearchVisible)
            {
                if (para == string.Empty)
                {
                    IsSearchVisible = false;
                    MusicList = tempMusic;
                    return;
                }
                //查找
                var searchedMusicList = MusicList.Where(m => m.Title.Contains(para, StringComparison.OrdinalIgnoreCase));
                if (searchedMusicList != null)
                {
                    MusicList = new ObservableCollection<Music>(searchedMusicList);
                }
            }
            else
            {
                IsSearchVisible = true;
            }
        }

        private void ToPlayMusic(Music Music)
        {
            if (isUpdating)
            {
                return;
            }
            isUpdating = true;
            //设置播放状态
            var playingMusic = MusicList.FirstOrDefault(
                m => m.Status == Music.PlayStatus.StartPlay
            );
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
            SelectedMusic = Music;
            SelectedMusic.Status = Music.PlayStatus.StartPlay;

            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(
                        MusicList,
                        MusicList.IndexOf(SelectedMusic),
                        "MainView"
                    )
                );
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Music, "LyricView"));
            isUpdating = false;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("journal"))
            {
                journal = navigationContext.Parameters.GetValue<IRegionNavigationJournal>("journal");
            }
            if (navigationContext.Parameters.ContainsKey("Id"))
            {
                playListId = navigationContext.Parameters.GetValue<Guid>("Id");

                //发现打开的是已加载的播放列表，则直接退出不必重新加载
                if (whichPlayList != Guid.Empty && playListId == whichPlayList)
                {
                    return;
                }
                whichPlayList = playListId;
                UpdateLoading(true);
                PlayList = null;
                var playLists = await playListService.GetByIdAsync(playListId);
                if (playLists != null)
                {
                    PlayList = playLists;
                }
                else
                {
                    eventAggregator.SendMessage("连接出现问题~~~");
                    UpdateLoading(false);
                    return;
                }
                MusicList = null; 
                var musics = await playListService.GetMusicsByPlayListIdAsync(playListId);
                if (musics != null)
                {
                    MusicList = new ObservableCollection<Music>(musics);
                    tempMusic = MusicList;
                    Count = MusicList.Count;
                    Duration = MusicList.Sum(m => m.Duration);
                }
                UpdateLoading(false);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { }

        #endregion
    }
}
