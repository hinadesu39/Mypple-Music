using MediatR;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Mypple_Music.ViewModels
{
    public class MainViewModel : NavigationViewModel
    {
        private bool isUpdating = false; //后台更新触发SelectionChanged后直接返回不执行命令
        public static MediaElement MediaElement;

        private readonly IMediator mediator;
        private readonly IRegionManager RegionManager;
        private readonly IDialogHostService dialog;
        private readonly IContainerProvider container;
        private IRegionNavigationJournal journal;

        //private readonly IDialogHostService dialog;

        #region 属性

        private int playIndex;

        public int PlayIndex
        {
            get { return playIndex; }
            set { playIndex = value; }
        }

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

        private Visibility volumeHigh = Visibility.Visible;

        public Visibility VolumeHigh
        {
            get { return volumeHigh; }
            set
            {
                volumeHigh = value;
                RaisePropertyChanged();
            }
        }

        private Visibility volumeMute = Visibility.Collapsed;

        public Visibility VolumeMute
        {
            get { return volumeMute; }
            set
            {
                volumeMute = value;
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

        private UserDto userDto;

        public UserDto UserDto
        {
            get { return userDto; }
            set
            {
                userDto = value;
                RaisePropertyChanged();
            }
        }

        private string userAvatar;

        public string UserAvatar
        {
            get { return userAvatar; }
            set
            {
                userAvatar = value;
                RaisePropertyChanged();
            }
        }

        private bool playOrPause;

        public bool PlayOrPause
        {
            get { return playOrPause; }
            set
            {
                playOrPause = value;
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

        private int menuBarsIndex = -1;

        public int MenuBarsIndex
        {
            get { return menuBarsIndex; }
            set
            {
                menuBarsIndex = value;
                RaisePropertyChanged();
            }
        }

        private int musicinfoIndex = -1;

        public int MusicInfoIndex
        {
            get { return musicinfoIndex; }
            set
            {
                musicinfoIndex = value;
                RaisePropertyChanged();
            }
        }

        private int playListIndex = -1;

        public int PlayListIndex
        {
            get { return playListIndex; }
            set
            {
                playListIndex = value;
                RaisePropertyChanged();
            }
        }
        #endregion 属性


        #region 命令
        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        public DelegateCommand<string> ExecuteCommand { get; set; }
        public DelegateCommand ConfigCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }
        public DelegateCommand UserCenterCommand { get; set; }
        public DelegateCommand LoginOutCommand { get; set; }
        public DelegateCommand PlayCommand { get; set; }
        public DelegateCommand<MediaElement> MediaLoadedCommand { get; set; }
        public DelegateCommand<string> MediaEndedCommand { get; set; }
        public DelegateCommand<object> VolumeValueChangedCommand { get; set; }
        public DelegateCommand MusicProgressChangedCommand { get; set; }
        public DelegateCommand<string> PlayModeChangedCommand { get; set; }
        public DelegateCommand<string> ChangeMusicCommand { get; set; }
        #endregion 命令

        #region 构造函数
        public MainViewModel(
            IDialogHostService dialog,
            IRegionManager regionManager,
            IContainerProvider Container,
            IMediator mediator
        )
            : base(Container)
        {
            this.mediator = mediator;
            this.dialog = dialog;
            this.RegionManager = regionManager;
            this.container = Container;

            MenuBars = new ObservableCollection<MenuBar>();
            MusicInfoBars = new ObservableCollection<MenuBar>();
            PlayListBars = new ObservableCollection<MenuBar>();

            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            ConfigCommand = new DelegateCommand(Config);
            GoBackCommand = new DelegateCommand(GoBack);
            UserCenterCommand = new DelegateCommand(goUserCenterAsync);
            ExecuteCommand = new DelegateCommand<string>(Execute);

            //创建播放器
            PlayCommand = new DelegateCommand(Play);
            MediaLoadedCommand = new DelegateCommand<MediaElement>(MediaLoadedAsync);
            MediaEndedCommand = new DelegateCommand<string>(MediaEnded);
            VolumeValueChangedCommand = new DelegateCommand<object>(VolumeValueChanged);
            MusicProgressChangedCommand = new DelegateCommand(MusicProgressChanged);
            PlayModeChangedCommand = new DelegateCommand<string>(PlayModeChanged);
            ChangeMusicCommand = new DelegateCommand<string>(ChangeMusic);
            //LoginOutCommand = new DelegateCommand(() =>
            //{
            //    App.LoginOut(Container);
            //});
            //this.dialog = dialog;

            //事件订阅
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Subscribe(
                    arg =>
                    {
                        PlayList = arg.Musics;
                        PlayIndex = arg.id;
                        InitPlay(PlayList[PlayIndex]);
                        MediaElement.Play();
                        PlayOrPause = true;
                    },
                    m =>
                    {
                        return m.filter == "MainView";
                    }
                );
        }

        #endregion 构造函数

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
                    break;
                case "RepeatOne":
                    Player.Mode = PlayerModel.PlayMode.RepeatOne;
                    break;
                case "PlayInOrder":
                    Player.Mode = PlayerModel.PlayMode.PlayInOrder;
                    break;
                case "True":
                    Player.Mode = PlayerModel.PlayMode.RepeatOne;
                    break;
                case "False":
                    Player.Mode = PlayerModel.PlayMode.PlayInOrder;
                    break;
            }
        }

        private void MusicProgressChanged()
        {
            Debug.WriteLine(Player.PlayProgress);
            MediaElement.Position = TimeSpan.FromSeconds(Player.PlayProgress);
        }

        private void VolumeValueChanged(object value)
        {
            Player.VolumeValue = Convert.ToDouble(value);
            MediaElement.Volume = Player.VolumeValue;
            Debug.WriteLine(value);
            if (Player.VolumeValue < 0.03)
            {
                Player.VolumeValue = 0;
                VolumeHigh = Visibility.Collapsed;
                VolumeMute = Visibility.Visible;
            }
            else
            {
                VolumeMute = Visibility.Collapsed;
                VolumeHigh = Visibility.Visible;
            }
        }

        /// <summary>
        /// 当前播放结束后作何动作
        /// </summary>
        private void MediaEnded(string obj = "Next")
        {
            //Player.Timer.Dispose();
            switch (Player.Mode)
            {
                case PlayerModel.PlayMode.ShufflePlay:
                    if (Player.Music != null)
                        Player.Music.Status = Music.PlayStatus.ClosePlay;
                    //创建一个随机数种子提高随机数不重复概率
                    byte[] buffer = Guid.NewGuid().ToByteArray();
                    int seed = BitConverter.ToInt32(buffer, 0);
                    Random random = new Random(seed);
                    int randomIndex = random.Next(0, PlayList.Count);
                    while (randomIndex == PlayIndex)
                    {
                        randomIndex = random.Next(0, PlayList.Count);
                    }
                    InitPlay(PlayList[randomIndex]);
                    Player.Music.Status = Music.PlayStatus.StartPlay;
                    break;
                case PlayerModel.PlayMode.PlayInOrder:
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
                        Player.Music.Status = Music.PlayStatus.ClosePlay;
                    InitPlay(PlayList[PlayIndex]);
                    Player.Music.Status = Music.PlayStatus.StartPlay;
                    break;
                case PlayerModel.PlayMode.RepeatOne:
                    InitPlay(Player.Music);
                    break;
            }
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Player.Music, "LyricView"));
        }

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
        private async void InitPlay(Music music)
        {
            //初始化控件数据
            Player.Music = music;
            PlayIndex = PlayList.IndexOf(music);
            Player.PlayProgress = 0;
            Player.PlayProgressLength = 1;
            MediaElement.Source = music.AudioUrl;
            Player.PlayProgressLength = music.Duration;
        }

        private void Play()
        {
            if (PlayOrPause)
            {
                MediaElement.Play();
            }
            else
            {
                MediaElement.Pause();
            }
        }

        private void goUserCenterAsync()
        {
            DialogParameters parameter = new DialogParameters();
            if (UserDto != null)
            {
                parameter.Add("Value", UserDto);
            }
            //var res = await dialog.ShowDialog("UserCenterView", parameter);
        }

        /// <summary>
        /// 返回之后要更新左侧导航栏的导航条
        /// </summary>
        private void GoBack()
        {
            if (!isUpdating)
                if (journal != null && journal.CanGoBack)
                {
                    journal.GoBack();
                    isUpdating = true;
                    var preNavi = MenuBars.FirstOrDefault(
                        m => m.NameSpace == journal.CurrentEntry.Uri.ToString()
                    );
                    if (preNavi != null)
                    {
                        MenuBarsIndex = MenuBars.IndexOf(preNavi);
                        PlayListIndex = -1;
                        MusicInfoIndex = -1;
                        isUpdating = false;
                        return;
                    }
                    preNavi = MusicInfoBars.FirstOrDefault(
                        m => m.NameSpace == journal.CurrentEntry.Uri.ToString()
                    );
                    if (preNavi != null)
                    {
                        MusicInfoIndex = MusicInfoBars.IndexOf(preNavi);
                        PlayListIndex = -1;
                        MenuBarsIndex = -1;
                        isUpdating = false;
                        return;
                    }
                    preNavi = PlayListBars.FirstOrDefault(
                        m => m.NameSpace == journal.CurrentEntry.Uri.ToString()
                    );
                    if (preNavi != null)
                    {
                        PlayListIndex = PlayListBars.IndexOf(preNavi);
                        MenuBarsIndex = -1;
                        MusicInfoIndex = -1;
                        isUpdating = false;
                        return;
                    }
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

            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                menu.NameSpace,
                Callback =>
                {
                    journal = Callback.Context.NavigationService.Journal;
                    isUpdating = true;
                    if (IsLyricViewAlive)
                        IsLyricViewAlive = false; //如果是从歌词界面跳转，要先关闭歌词界面
                    isUpdating = false;
                }
            );
            //更新导航条
            switch (menu.BelongsTo)
            {
                case "MenuBars":
                    PlayListIndex = -1;
                    MusicInfoIndex = -1;
                    break;
                case "PlayListBars":
                    MenuBarsIndex = -1;
                    MusicInfoIndex = -1;
                    break;
                case "MusicInfoBars":
                    PlayListIndex = -1;
                    MenuBarsIndex = -1;
                    break;
            }
        }

        /// <summary>
        /// 泛用命令
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "LyricView":
                    if (MediaElement.Source != null)
                    {
                        NavigationParameters para = new NavigationParameters
                        {
                            {
                                "LyricInfo",
                                new LyricCreatedModel(
                                    new List<Music>(PlayList),
                                    PlayIndex,
                                    MediaElement
                                )
                            }
                        };
                        RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                            obj.ToString(),
                            para
                        );
                    }
                    break;
                case "Plus": //弹出添加播放列表对话框
                    AddPlayList();
                    break;
                case "TurnOffLyricview":
                    if (!IsLyricViewAlive)
                    {
                        GoBack();
                    }
                    break;
            }
        }

        private async void AddPlayList()
        {
            DialogParameters parameter = new DialogParameters();
            var dialogRes = await dialog.ShowDialog("AddPlayListView", parameter);
        }

        void CreateMenuBar()
        {
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "MotionPlayOutline",
                    Title = "现在就听",
                    NameSpace = "NowToListenView",
                    BelongsTo = "MenuBars"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "ViewGridOutline",
                    Title = "浏览",
                    NameSpace = "BrowserView",
                    BelongsTo = "MenuBars"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "Broadcast",
                    Title = "广播",
                    NameSpace = "BroadcastView",
                    BelongsTo = "MenuBars"
                }
            );
            MenuBars.Add(
                new MenuBar()
                {
                    Icon = "Cog",
                    Title = "设置",
                    NameSpace = "SettingsView",
                    BelongsTo = "MenuBars"
                }
            );

            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "ClockTimeNineOutline",
                    Title = "最近添加",
                    NameSpace = "RecentPostsView",
                    BelongsTo = "MusicInfoBars"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "AccountMusicOutline",
                    Title = "艺人",
                    NameSpace = "ArtistView",
                    BelongsTo = "MusicInfoBars"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "Album",
                    Title = "专辑",
                    NameSpace = "AlbumView",
                    BelongsTo = "MusicInfoBars"
                }
            );
            MusicInfoBars.Add(
                new MenuBar()
                {
                    Icon = "MusicNoteOutline",
                    Title = "歌曲",
                    NameSpace = "MusicListView",
                    BelongsTo = "MusicInfoBars"
                }
            );

            PlayListBars.Add(
                new MenuBar()
                {
                    Icon = "Drag",
                    Title = "所有播放列表",
                    NameSpace = "AllPlayListsView",
                    BelongsTo = "PlayListBars"
                }
            );

            PlayListBars.Add(
                new MenuBar()
                {
                    Icon = "CardsHeartOutline",
                    Title = "我的最爱",
                    NameSpace = "PlayListView",
                    BelongsTo = "PlayListBars"
                }
            );
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Config()
        {
            if (MenuBars.Count == 0)
            {
                CreateMenuBar();
            }
            //UserDto = AppSession.UserDto;
            //UserAvatar = AppSession.UserAvatar;
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(
                "RecentPostsView"
            );
        }
    }
}
