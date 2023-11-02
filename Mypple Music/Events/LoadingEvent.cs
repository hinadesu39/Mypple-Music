using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Events
{
    public class LoadingEvent : PubSubEvent<LoadingModel> { }

    public record LoadingModel(bool IsLoading, string filter = "MainView");
}
