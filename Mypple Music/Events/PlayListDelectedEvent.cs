using Mypple_Music.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Events
{
    public class PlayListDeletedEvent : PubSubEvent<PlayListDeletedModel>
    {
    }

    public record PlayListDeletedModel(PlayList PlayList,string filter = "MainView");
}
