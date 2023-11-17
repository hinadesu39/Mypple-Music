using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Events
{
    public class MessageEvent : PubSubEvent<MessageModel>
    {
    }

    public record MessageModel(string Message,string Filter);
}
