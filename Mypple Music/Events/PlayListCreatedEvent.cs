using Mypple_Music.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Events
{
    public class PlayListCreatedEvent: PubSubEvent<PlayListCreatedModel>
    {
    }

    public record PlayListCreatedModel(ObservableCollection<Music> Musics, int id, string filter);

}
