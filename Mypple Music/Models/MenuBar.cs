using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class MenuBar : BindableBase
    {
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string nameSpace;

        public string NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }

        private string belongsTo;

        public string BelongsTo
        {
            get { return belongsTo; }
            set { belongsTo = value; }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(); }
        }
    }
}
