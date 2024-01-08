using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public record SearchedResult(Music?[] musics, Album?[] albums, Artist?[] artists);
}
