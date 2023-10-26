using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class BroadcastViewModel : NavigationViewModel
    {
        public BroadcastViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
        }
    }
}
