using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class Lyric:BindableBase
    {
		//歌词原句
		private string content;

		public string Content
        {
			get { return content; }
			set { content = value; RaisePropertyChanged(); }
		}

		//歌词时间
		private double timeSpan;

		public double TimeSpan
        {
			get { return timeSpan; }
			set { timeSpan = value; }
		}

		//歌词翻译
		private string translation;

		public string Translation
        {
			get { return translation; }
			set { translation = value; }
		}



	}
}
