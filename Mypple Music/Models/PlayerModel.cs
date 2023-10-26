using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Mypple_Music.Models
{
    public class PlayerModel:BindableBase
    {
        /// <summary>
        /// 当前播放器音量
        /// </summary>
        private double volumeValue;
        public double VolumeValue
        {
            get { return volumeValue; }
            set
            { volumeValue = value; RaisePropertyChanged(); }

        }
        /// <summary>
        /// 当前播放歌曲
        /// </summary>
        private Music music;

        public Music Music
        {
            get { return music; }
            set { music = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 更新进度条定时器
        /// </summary>
        private PeriodicTimer timer;

        public PeriodicTimer Timer
        {
            get { return timer; }
            set { timer = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 歌曲进度条进度
        /// </summary>
        private double playProgress;
        public double PlayProgress
        {
            get { return playProgress; }
            set
            {
                playProgress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 歌曲进度条结束进度
        /// </summary>
        private double playEndProgress;
        public double PlayEndProgress
        {
            get { return playEndProgress; }
            set
            {
                playEndProgress = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 歌曲进度条长度
        /// </summary>
        private double playProgressLength;
        public double PlayProgressLength
        {
            get { return playProgressLength; }
            set
            {
                playProgressLength = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 播放模式
        /// </summary>
        private PlayMode mode = PlayMode.PlayInOrder;

        public PlayMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }


        public enum PlayMode 
        {
            RepeatOne,
            ShufflePlay,
            PlayInOrder
        }


    }
}
