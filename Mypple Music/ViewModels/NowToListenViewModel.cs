using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class NowToListenViewModel : NavigationViewModel
    {
        public NowToListenViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {

        }
    }

}
