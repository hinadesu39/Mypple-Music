using Mypple_Music.Common;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace Mypple_Music.ViewModels
{
    public class MainViewModel : NavigationViewModel
    {
        #region Field
        private bool isUpdating = false; //后台更新触发SelectionChanged后直接返回不执行命令
        public static MediaElement MediaElement;
        private readonly IPlayListService playListService;
        private readonly ILoginService loginService;
        private readonly IRegionManager RegionManager;
        private readonly IDialogHostService dialog;
        private readonly IContainerProvider container;
        private readonly ILogger logger;
        private IRegionNavigationJournal journal;
        private ObservableCollection<MenuBar> AllBars;
        #endregion

        #region Property

        private int playIndex;

        public int PlayIndex
        {
            get { return playIndex; }
            set { playIndex = value; }
        }

        //原始清单
        private ObservableCollection<Music> playList;

        public ObservableCollection<Music> PlayList
        {
            get { return playList; }
            set
            {
                playList = value;
                RaisePropertyChanged();
            }
        }

        //待播清单
        private ObservableCollection<Music> toPlayList;

        public ObservableCollection<Music> ToPlayList
        {
            get { return toPlayList; }
            set
            {
                toPlayList = value;
                RaisePropertyChanged();
            }
        }

        //随机播放清单
        private ObservableCollection<Music> shufflePlayList;

        public ObservableCollection<Music> ShufflePlayList
        {
            get { return shufflePlayList; }
            set
            {
                shufflePlayList = value;
                RaisePropertyChanged();
            }
        }

        //历史播放清单
        private ObservableCollection<Music> historyPlayList;

        public ObservableCollection<Music> HistoryPlayList
        {
            get { return historyPlayList; }
            set
            {
                historyPlayList = value;
                RaisePropertyChanged();
            }
        }

        private PlayerModel player;

        public PlayerModel Player
        {
            get { return player; }
            set
            {
                player = value;
                RaisePropertyChanged();
            }
        }

        private SimpleUser userDto;

        public SimpleUser UserDto
        {
            get { return userDto; }
            set
            {
                userDto = value;
                RaisePropertyChanged();
            }
        }

        private bool isLyricViewAlive;

        public bool IsLyricViewAlive
        {
            get { return isLyricViewAlive; }
            set
            {
                isLyricViewAlive = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set
            {
                menuBars = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<MenuBar> musicInfoBars;

        public ObservableCollection<MenuBar> MusicInfoBars
        {
            get { return musicInfoBars; }
            set
            {
                musicInfoBars = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<MenuBar> playListBars;

        public ObservableCollection<MenuBar> PlayListBars
        {
            get { return playListBars; }
            set
            {
                playListBars = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        public DelegateCommand<string> ExecuteCommand { get; set; }
        public DelegateCommand InitCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }
        public DelegateCommand UserCenterCommand { get; set; }
        public DelegateCommand LoginOutCommand { get; set; }
        public DelegateCommand<MediaElement> MediaLoadedCommand { get; set; }
        public DelegateCommand<string> MediaEndedCommand { get; set; }
        public DelegateCommand<object> VolumeValueChangedCommand { get; set; }
        public DelegateCommand MusicProgressChangedCommand { get; set; }
        public DelegateCommand<string> PlayModeChangedCommand { get; set; }
        public DelegateCommand<string> ChangeMusicCommand { get; set; }
        public DelegateCommand<Music> RemoveMusicCommand { set; get; }
        public DelegateCommand<object> ToPlayMusicCommand { get; set; }
        public DelegateCommand ClearToPlayListCommand { set; get; }
        public DelegateCommand LocateMusicCommand { get; set; }
        public DelegateCommand<string> SearchCommand { get; set; }
        #endregion

        #region Ctor
        public MainViewModel(
            IDialogHostService dialog,
            IRegionManager regionManager,
            IContainerProvider Container,
            IPlayListService playListService,
            ILoginService loginService,
            ILogger logger
        )
            : base(Container)
        {
            this.logger = logger;
            logger.Information("Success");
            this.dialog = dialog;
            this.RegionManager = regionManager;
            this.container = Container;
            this.playListService = playListService;
            this.loginService = loginService;
            MenuBars = new ObservableCollection<MenuBar>();
            MusicInfoBars = new ObservableCollection<MenuBar>();
            PlayListBars = new ObservableCollection<MenuBar>();
            HistoryPlayList = new ObservableCollection<Music>();

            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            InitCommand = new DelegateCommand(Init);
            
            GoBackCommand = new DelegateCommand(GoBack);
            ExecuteCommand = new DelegateCommand<string>(Execute);
            RemoveMusicCommand = new DelegateCommand<Music>(RemoveMusic);
            ToPlayMusicCommand = new DelegateCommand<object>(ToPlayMusic);
            ClearToPlayListCommand = new DelegateCommand(ClearToPlayList);
            LocateMusicCommand = new DelegateCommand(LocateMusic);
            SearchCommand = new DelegateCommand<string>(Search);

            //播放相关
            MediaLoadedCommand = new DelegateCommand<MediaElement>(MediaLoadedAsync);
            MediaEndedCommand = new DelegateCommand<string>(MediaEnded);
            VolumeValueChangedCommand = new DelegateCommand<object>(VolumeValueChanged);
            MusicProgressChangedCommand = new DelegateCommand(MusicProgressChanged);
            PlayModeChangedCommand = new DelegateCommand<string>(PlayModeChanged);
            ChangeMusicCommand = new DelegateCommand<string>(ChangeMusic);

            //播放列表创建事件订阅
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Subscribe(
                    arg =>
                    {
                        //将当前播放音乐状态设置成关闭
                        if (Player.Music != null)
                            Player.Music.Status = Music.PlayStatus.StopPlay;
                        PlayList = arg.Musics;

                        //如果提前点击了随机播放按钮，那么收到播放列表时应当及时生成随机列表
                        if (Player.Mode == PlayerModel.PlayMode.ShufflePlay)
                        {
                            ShufflePlayList = new ObservableCollection<Music>(PlayList);
                            Shuffle(ShufflePlayList);
                            PlayIndex = ShufflePlayList.IndexOf(Player.Music);
                            ToPlayList = ShufflePlayList;
                        }
                        else
                        {
                            ToPlayList = new ObservableCollection<Music>(arg.Musics);
                            PlayIndex = arg.id;
                        }
                        InitPlay(ToPlayList[PlayIndex]);
                        MediaElement.Play();
                    },
                    m =>
                    {
                        return m.filter == "MainView";
                    }
                );
            //播放状态变化事件订阅
            eventAggregator
                .GetEvent<MusicPlayStatusChangedEvent>()
                .Subscribe(
                    arg =>
                    {
                        if (arg.Status == Music.PlayStatus.StartPlay)
                        {
                            MediaElement.Play();
                        }
                        else if (arg.Status == Music.PlayStatus.PausePlay)
                        {
                            MediaElement.Pause();
                        }
                    },
                    m =>
                    {
                        return m.filter == "MainView";
                    }
                );
        }


        #endregion

        #region Command

        private void Search(string obj)
        {
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("SearchView");
        }

        /// <summary>
        /// 在播放列表中定位到当前播放的音乐
        /// </summary>
        private void LocateMusic()
        {
            //重置Player.Music的引用以触发前台SelectionChanged命令
            var tempMusic = Player.Music;
            Player.Music = null;
            Player.Music = tempMusic;
        }

        /// <summary>
        /// 清空待播列表
        /// </summary>
        private void ClearToPlayList()
        {
            ToPlayList.Clear();
        }

        /// <summary>
        /// 双击待播列表播放音乐
        /// </summary>
        /// <param name="obj"></param>
        private void ToPlayMusic(object obj)
        {
            PlayIndex = (int)obj;
            //改变歌曲播放状态
            if (Player.Music != null)
                Player.Music.Status = Music.PlayStatus.StopPlay;
            InitPlay(ToPlayList[PlayIndex]);
            Player!.Music!.Status = Music.PlayStatus.StartPlay;

            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Player.Music, "LyricView"));
        }

        /// <summary>
        /// 在待播列表中移除歌曲
        /// </summary>
        /// <param name="music"></param>
        private void RemoveMusic(Music music)
        {
            ToPlayList.Remove(music);
        }

        /// <summary>
        /// 上一首或下一首
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeMusic(string obj)
        {
            if (PlayList != null)
            {
                MediaEnded(obj);
            }
        }

        /// <summary>
        /// 当点击随机播放时强制随机播放，不管循环状态，
        /// 取消随机播放时让播放状态回归当前循环状态，
        /// True为RepeatOne，False为PlayInOrder
        /// </summary>
        /// <param name="obj"></param>
        private void PlayModeChanged(string obj)
        {
            switch (obj)
            {
                case "ShufflePlay":
                    Player.Mode = PlayerModel.PlayMode.ShufflePlay;
                    if (PlayList != null)
                    {
                        ShufflePlayList = new ObservableCollection<Music>(PlayList);
                        Shuffle(ShufflePlayList);
                        PlayIndex = ShufflePlayList.IndexOf(Player.Music);
                        ToPlayList = ShufflePlayList;
                    }
                    break;
                case "RepeatOne":
                    Player.Mode = PlayerModel.PlayMode.RepeatOne;
                    break;
                case "PlayInOrder":
                    Player.Mode = PlayerModel.PlayMode.PlayInOrder;
                    if (PlayList != null)
                    {
                        PlayIndex = PlayList.IndexOf(Player.Music);
                        ToPlayList = PlayList;
                    }
                    break;
                case "True":
                    Player.Mode = PlayerModel.PlayMode.RepeatOne;
                    break;
                case "False":
                    Player.Mode = PlayerModel.PlayMode.PlayInOrder;
                    if (PlayList != null)
                    {
                        PlayIndex = PlayList.IndexOf(Player.Music);
                        ToPlayList = PlayList;
                    }
                    break;
            }
            LocateMusic();
        }

        /// <summary>
        /// 播放进度改变
        /// </summary>
        private void MusicProgressChanged()
        {
            Debug.WriteLine(Player.PlayProgress);
            MediaElement.Position = TimeSpan.FromSeconds(Player.PlayProgress);
        }

        /// <summary>
        /// 音量改变
        /// </summary>
        /// <param name="value"></param>
        private void VolumeValueChanged(object value)
        {
            Player.VolumeValue = Convert.ToDouble(value);
            MediaElement.Volume = Player.VolumeValue;
            Debug.WriteLine(value);
            if (Player.VolumeValue < 0.03)
            {
                Player.VolumeValue = 0;
            }
        }

        /// <summary>
        /// 当前播放结束后作何动作
        /// </summary>
        private void MediaEnded(string obj = "Next")
        {
            switch (Player.Mode)
            {
                //随机or顺序播放
                case PlayerModel.PlayMode.ShufflePlay
                or PlayerModel.PlayMode.PlayInOrder:

                    if (obj == "Pre")
                    {
                        PlayIndex = PlayIndex == 0 ? PlayList.Count - 1 : PlayIndex - 1;
                    }
                    else
                    {
                        PlayIndex = PlayIndex == PlayList.Count - 1 ? 0 : PlayIndex + 1;
                    }
                    //改变歌曲播放状态
                    if (Player.Music != null)
                        Player.Music.Status = Music.PlayStatus.StopPlay;
                    if (ToPlayList.Count != 0)
                        InitPlay(ToPlayList[PlayIndex]);
                    Player!.Music!.Status = Music.PlayStatus.StartPlay;
                    break;
                case PlayerModel.PlayMode.RepeatOne:
                    MediaElement.Position = TimeSpan.Zero;
                    break;
            }
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Player.Music, "LyricView"));
        }

        /// <summary>
        /// 播放器加载
        /// </summary>
        /// <param name="mediaElement"></param>
        private async void MediaLoadedAsync(MediaElement mediaElement)
        {
            MediaElement = mediaElement;
            //初始化播放信息
            Player = new PlayerModel();
            Player.VolumeValue = 1;
            //计时器更新进度条
            Player.Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
            while (await Player.Timer.WaitForNextTickAsync())
            {
                //播放进度
                var totalSeconds = MediaElement.Position.TotalSeconds;
                //更新进度条
                Player.PlayProgress = totalSeconds;
                Player.PlayEndProgress = Player.PlayProgressLength - totalSeconds;
            }
        }

        /// <summary>
        /// 准备播放歌曲
        /// </summary>
        /// <param name="music"></param>
        private void InitPlay(Music music)
        {
            //如果发现播放的是同一首歌
            if (music.AudioUrl == MediaElement.Source)
            {
                MediaElement.Position = TimeSpan.Zero;
                return;
            }

            //初始化控件数据
            Player.Music = music;
            Player.Music.Status = Music.PlayStatus.StartPlay;
            PlayIndex = ToPlayList.IndexOf(music);
            Player.PlayProgress = 0;
            MediaElement.Source = DownloadHelper.GetMusicPath(music.AudioUrl);
            Player.PlayProgressLength = music.Duration;

            //添加到历史播放列表
            HistoryPlayList.Add(music);
        }

        /// <summary>
        /// 返回之后要更新左侧导航栏的导航条
        /// </summary>
        private void GoBack()
        {
            if (!isUpdating)

                if (journal != null && journal.CanGoBack)
                {
                    var preNavi = AllBars.FirstOrDefault(
                        m => m.NameSpace == journal.CurrentEntry.Uri.ToString()
                    );
                    if (preNavi != null)
                        preNavi.IsSelected = false;

                    journal.GoBack();

                    isUpdating = true;
                    if (IsLyricViewAlive) //如果从歌词界面返回则及时关闭歌词按钮效果
                        IsLyricViewAlive = false;

                    var curNavi = AllBars.FirstOrDefault(
                        m => m.NameSpace == journal.CurrentEntry.Uri.ToString()
                    );

                    if (curNavi != null)
                        curNavi.IsSelected = true;
                    isUpdating = false;
                }
        }

        /// <summary>
        /// 导航命令
        /// </summary>
        /// <param name="obj"></param>
        private void Navigate(MenuBar menu)
        {
            if (menu == null || isUpdating)
                return;
            var preNavi = AllBars?.FirstOrDefault(m => m.IsSelected == true && m != menu);
            if (preNavi != null)
                preNavi.IsSelected = false;

            NavigationParameters para = new NavigationParameters();
            para.Add("Title", menu.Title);
            para.Add("Id", menu.Id);
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                menu.NameSpace,
                Callback =>
                {
                    journal = Callback.Context.NavigationService.Journal;
                    isUpdating = true;
                    if (IsLyricViewAlive)
                        IsLyricViewAlive = false; //如果是从歌词界面跳转，要先关闭歌词界面
                    isUpdating = false;
                },
                para
            );
        }

        /// <summary>
        /// 泛用命令
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Execute(string obj)
        {
            if (obj == "LyricView" && MediaElement.Source != null)
            {
                NavigationParameters para = new NavigationParameters();
                para.Add("LyricInfo", new LyricCreatedModel(Player.Music, MediaElement));
                RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                    obj.ToString(),
                    para
                );
            }
            else if (obj == "Plus") //弹出添加播放列表对话框
            {
                AddPlayList();
            }
            else if (obj == "TurnOffLyricview" && !IsLyricViewAlive && !isUpdating)
            {
                GoBack();
            }
            else if (obj == "登录/切换用户")
            {
                OpenLoginView();
            }
            else if (obj == "UserCenterView")
            {
                NavigationParameters para = new NavigationParameters();
                para.Add("User", UserDto);
                para.Add("journal", journal);
                RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                    obj.ToString(),
                    Callback =>
                    {
                        journal = Callback.Context.NavigationService.Journal;
                    },
                    para
                );
            }
            else if (obj == "注销")
            {
                AppSession.JWTToken = "";
                PlayListBars = new ObservableCollection<MenuBar>(PlayListBars.Take(2));
                UserDto = null;
            }
        }

        /// <summary>
        /// 打开登录界面
        /// </summary>
        private async void OpenLoginView()
        {
            DialogParameters para = new DialogParameters();
            var dialogRes = await dialog.ShowDialog("LoginView", para);
            if (dialogRes.Result == ButtonResult.OK)
            {
                if (dialogRes.Parameters.ContainsKey("User"))
                {
                    UserDto = dialogRes.Parameters.GetValue<SimpleUser>("User");
                    PlayListBars = new ObservableCollection<MenuBar>(PlayListBars.Take(2));
                    var playList = await playListService.GetAllAsync();
                    if (playList != null)
                        foreach (var item in playList)
                        {
                            PlayListBars.Add(
                                new MenuBar()
                                {
                                    Id = item.Id,
                                    Icon = "PlaylistMusicOutline",
                                    Title = item.Title,
                                    NameSpace = "PlayListView"
                                }
                            );
                        }
                }
            }
        }

        /// <summary>
        /// 添加播放列表
        /// </summary>
        private async void AddPlayList()
        {
            DialogParameters parameter = new DialogParameters();
            var dialogRes = await dialog.ShowDialog("AddPlayListView", parameter);
            if (dialogRes.Result == ButtonResult.OK)
            {
                var playList = dialogRes.Parameters.GetValue<PlayList>("Value");
                Debug.WriteLine(playList.PicUrl);
                playList.PicUrl = await playListService.UploadAsync(playList.LocalPicUrl);
                var res = await playListService.AddPlayListAsync(
                    new PlayListAddRequest(playList.PicUrl, playList.Title, playList.Description)
                );
                if (res != null)
                {
                    eventAggregator.SendMessage("添加成功");
                }
                else
                {
                    eventAggregator.SendMessage("添加失败");
                }
                PlayListBars.Add(
                    new MenuBar()
                    {
                        Id = res!.Id,
                        Icon = "PlaylistMusicOutline",
                        Title = res.Title,
                        NameSpace = "PlayListView"
                    }
                );
            }
        }

        /// <summary>
        /// 创建导航菜单
        /// </summary>
        void CreateMenuBar()
        {
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "MotionPlayOutline",
                    Title = "现在就听",
                    NameSpace = "NowToListenView"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "ViewGridOutline",
                    Title = "浏览",
                    NameSpace = "BrowserView"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "Cog",
                    Title = "设置",
                    NameSpace = "SettingsView"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "ClockTimeNineOutline",
                    Title = "最近添加",
                    NameSpace = "RecentPostsView",
                    IsSelected = true
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "AccountMusicOutline",
                    Title = "艺人",
                    NameSpace = "ArtistView"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "Album",
                    Title = "专辑",
                    NameSpace = "AlbumView"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "MusicNoteOutline",
                    Title = "歌曲",
                    NameSpace = "MusicListView"
                }
            );

            PlayListBars.Add(
                new MenuBar()
                {
                    Icon = "Drag",
                    Title = "所有播放列表",
                    NameSpace = "AllPlayListsView"
                }
            );
            PlayListBars.Add(
                new MenuBar()
                {
                    Icon = "CardsHeartOutline",
                    Title = "我的最爱",
                    NameSpace = "PlayListView"
                }
            );
            //将所有导航菜单归并到一个集合当中
            AllBars = new ObservableCollection<MenuBar>(
                MenuBars.Concat(MusicInfoBars).Concat(PlayListBars)
            );
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async void Init()
        {
            if (MenuBars.Count == 0)
            {
                CreateMenuBar();
            }
            UpdateLoading(true);
            if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsAutoLogin")))
            {
                AppSession.JWTToken = ConfigurationManager.AppSettings.Get("JWTToken")!;
                UserDto = await loginService.GetUserInfo();
            }
            var playList = await playListService.GetAllAsync();
            if (playList != null)
            {
                foreach (var item in playList)
                {
                    var newBar = new MenuBar()
                    {
                        Id = item.Id,
                        Icon = "PlaylistMusicOutline",
                        Title = item.Title,
                        NameSpace = "PlayListView"
                    };
                    PlayListBars.Add(newBar);
                    AllBars.Add(newBar);
                }
                AppSession.AllPlayLists = new ObservableCollection<PlayList>(playList);
            }
            UpdateLoading(false);
            //RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
            //    "RecentPostsView",
            //    Callback =>
            //    {
            //        journal = Callback.Context.NavigationService.Journal;
            //    }
            //);
        }

        /// <summary>
        /// 洗牌，用于生成随机播放列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        static void Shuffle<T>(IList<T> list)
        {
            // 创建一个随机数生成器
            Random random = new Random();

            // 从列表的最后一个元素开始，向前遍历
            for (int i = list.Count - 1; i > 0; i--)
            {
                // 随机选择一个索引，范围是[0, i]
                int j = random.Next(i + 1);

                // 交换索引为i和j的元素
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        #endregion
    }
}
