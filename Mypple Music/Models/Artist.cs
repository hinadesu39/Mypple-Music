using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class Artist:BindableBase
    {
        private Guid id;

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 歌手图片
        /// </summary>
        private Uri? picUrl;
        public Uri? PicUrl
        {
            get { return picUrl; }
            set
            {
                picUrl = value;
                RaisePropertyChanged();
            }
        }

        private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}


	}
}
