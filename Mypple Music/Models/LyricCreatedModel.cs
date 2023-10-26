using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.Models
{
    public record LyricCreatedModel(List<Music> PlayList, int PlayIndex, MediaElement MediaElement);
}
