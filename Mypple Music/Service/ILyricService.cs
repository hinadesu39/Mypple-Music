using Mypple_Music.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public interface ILyricService
    {
        public ObservableCollection<Lyric> LyricSplitter(string lyric);
    }
}
