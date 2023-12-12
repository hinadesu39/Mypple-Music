using Mypple_Music.Models;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace Mypple_Music.Events
{
    public class AppSession
    {
        public static IEventAggregator EventAggregator;

        public static bool IsDarkTheme;

        public static string JWTToken;

    }
}
